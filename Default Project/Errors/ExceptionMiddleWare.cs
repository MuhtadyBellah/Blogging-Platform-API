using Microsoft.AspNetCore.Authentication;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Default_Project.Errors
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleWare> log;
        private readonly IHostEnvironment env;

        public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> log, IHostEnvironment env)
        {
            this.next = next;
            this.log = log;
            this.env = env;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;
            var query = context.Request.QueryString;
            var StatusCode = context.Response.StatusCode;

            try
            {
                var stopWatch = Stopwatch.StartNew();
                await next.Invoke(context);
                stopWatch.Stop();

                var user = context.User.Identity?.Name ?? "Anonymous";
                log.LogInformation($"{DateTime.Now} Request: {method} {path} // {query} // {user} // {stopWatch}ms => {StatusCode}");
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internal Server Error");
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
