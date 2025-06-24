using Default_Project.Cores.Interfaces;
using Default_Project.Services;
using Default_Project.Cores;
using Default_Project.Errors;
using Default_Project.Helper;
using Default_Project.Repos.Data;
using Default_Project.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Supabase;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Default_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Config Services - Add services to the container.
            builder.Services
                .AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Blogging Demo",
                    Version = "v1"
                });

                // Define the security scheme
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and the Sanctum token.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http, // Correct Type for Bearer Token
                    Scheme = "Bearer",             // Ensure this matches the Bearer scheme0
                    BearerFormat = "JWT"           // Optional, for display purposes
                });

                // Add security requirements
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            #region Connection
            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                    .EnableDetailedErrors();
            });
            #endregion

            //Controller
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>))
                            .AddScoped<ICache, CacheService>()
                            .AddScoped<IUnitWork, UnitWork>()
                            
                            //Redis Conn
                            .AddSingleton<IConnectionMultiplexer>(options =>
                            {
                                var conn = builder.Configuration.GetConnectionString("RedisConnection");
                                if (string.IsNullOrEmpty(conn))
                                {
                                    throw new InvalidOperationException("Redis connection string is not configured.");
                                }
                                return ConnectionMultiplexer.Connect(conn);
                            })
                            .AddSingleton(provider =>
                            {
                                var supabaseUrl = builder.Configuration["SupaBaseClient:Url"];
                                var supabaseKey = builder.Configuration["SupaBaseClient:Key"];
                                if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
                                {
                                    throw new InvalidOperationException("Supabase configuration is missing in appsettings.json.");
                                }
                                return new Client(supabaseUrl, supabaseKey);
                            })
                            .AddAutoMapper(typeof(MappingProfiles));

            //Validation
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray();
                    var result = new ValidationResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(result);
                };
            });
          
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", a =>
                {
                    a.SetIsOriginAllowed(origin => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
                options.AddPolicy("deepSeek", a =>
                {
                    a.WithOrigins("https://localhost:7182", "http://localhost:7182", "https://rational-deep-dinosaur.ngrok-free.app") // Your frontend URL
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials(); // Required for cookies/auth
                });
            });
            #endregion

            var app = builder.Build();

            #region Update-Database && DataSeed
            var scope = app.Services.CreateScope();
            var Services = scope.ServiceProvider;
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {
                var dbContext = Services.GetRequiredService<StoreContext>();
                await dbContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(dbContext);
            }
            catch (Exception ex)
            {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, $"An Error Occured During The Migration");
            }
            #endregion

            #region Config - Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                
            }

            app.UseExceptionHandler("/errors/500");
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            //app.UseHsts();
            app.UseHttpsRedirection();

            // Picture
            //app.UseStaticFiles();
            //app.UseCookiePolicy();
            app.UseRouting();
            app.UseCors("deepSeek");

            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseSession();
            app.UseMiddleware<ExceptionMiddleWare>();
            app.MapControllers();
            #endregion

            #region NGrok
            Process _ngrokProcess;
            _ngrokProcess = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\Ngrok\ngrok.exe",
                    Arguments = $"http --url=rational-deep-dinosaur.ngrok-free.app https://localhost:7182",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                _ngrokProcess.Start();
                Console.WriteLine("Starting ngrok process...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start ngrok process: {ex.Message}");
                if (_ngrokProcess != null && !_ngrokProcess.HasExited)
                {
                    _ngrokProcess.Kill();
                    _ngrokProcess.Dispose();
                    Console.WriteLine("Ngrok process terminated.");
                }
            }
            #endregion
            app.Run();
        }
    }
}
