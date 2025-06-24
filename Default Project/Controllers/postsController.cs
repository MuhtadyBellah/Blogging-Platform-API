using Microsoft.EntityFrameworkCore;
using Default_Project.Cores.Models;
using Default_Project.Cores;
using Default_Project.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Default_Project.Cores.Specifications;
using Default_Project.Errors;

namespace Default_Project.Controllers
{
    public class postsController : ApiBaseController
    {
        private readonly IUnitWork _context;
        private readonly IMapper _mapper;

        public postsController(IUnitWork context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlogDTO>), 200)]
        public async Task<ActionResult<IEnumerable<BlogDTO>>> GetBlogs([FromQuery] BlogSpecParams param)
        {
            var spec = new BlogSpecific(param);
            var blogs = await _context.Repo<Blog>().GetAllAsync(spec);
            var returned = _mapper.Map<IEnumerable<BlogDTO>>(blogs);
            return Ok(returned);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BlogDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<BlogDTO>> GetBlog(int id)
        {
            var blog = await _context.Repo<Blog>().GetBySpecAsync(new BlogSpecific(id));
            return blog == null ?
                NotFound(new ApiResponse(404)) :
                Ok(_mapper.Map<BlogDTO>(blog));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BlogDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<BlogDTO>> PutBlog(int id, BlogRequest blog)
        {
            if (id < 1 || blog is null || blog.tags is null)
                return BadRequest(new ApiResponse(400));

            var existBlog = await _context.Repo<Blog>().GetBySpecAsync(new BlogSpecific(id));
            if(existBlog == null) 
                return NotFound(new ApiResponse(404));
            
            existBlog.updatedAt = DateTimeOffset.UtcNow;
            existBlog.Category = blog.category;
            existBlog.Content = blog.content;
            existBlog.Title = blog.title;

            try
            {
                _context.Repo<Blog>().Update(existBlog);
                var oldHasRelations = await _context.Repo<Has>().GetAllAsync(new BaseSpecification<Has>(h => h.BlogId == id));
                foreach (var _ in oldHasRelations)
                    _context.Repo<Has>().Delete(_); 
                await _context.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _context.DisposeAsync();
                return BadRequest(new ApiResponse(500, ex.Message));
            }
            
            var newData = new List<Has>();
            foreach (var name in blog.tags.Distinct())
            {
                var tag = await _context.Repo<Tag>().GetBySpecAsync(new BaseSpecification<Tag>(t => t.Name == name));
                if (tag is null)
                    tag = new Tag() { Name = name };
                
                var newHas = new Has
                {
                    BlogId = id,
                    TagId =  tag.Id,
                };
                newData.Add(newHas);
            }

            try
            {
                await _context.Repo<Has>().AddRangeAsync(newData);
                await _context.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _context.DisposeAsync();
                return BadRequest(new ApiResponse(500, ex.Message));
            }

            return Ok(_mapper.Map<BlogDTO>(existBlog));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<BlogDTO>> PostBlog(BlogRequest blog)
        {
            if (blog == null)
                return BadRequest(new ApiResponse(400));

            var existingBlog = await _context.Repo<Blog>().GetBySpecAsync(new BaseSpecification<Blog>(b => b.Title == blog.title && b.Category == blog.category));
            if (existingBlog != null)
                return BadRequest(new ApiResponse(400, "Blog already exists."));
            
            var newBlog = new Blog { Category = blog.category, Content = blog.content, Title = blog.title };
            try
            {
                await _context.Repo<Blog>().AddAsync(newBlog);
                await _context.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _context.DisposeAsync();
                return BadRequest(new ApiResponse(500, ex.Message));
            }

            var newData = new List<Has>();
            foreach (var name in blog.tags.Distinct())
            {
                var tag = await _context.Repo<Tag>().GetBySpecAsync(new BaseSpecification<Tag>(t => t.Name == name));
                if (tag is null)
                {
                    tag = new Tag { Name = name };
                    try
                    {
                        await _context.Repo<Tag>().AddAsync(tag);
                        await _context.CompleteAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        await _context.DisposeAsync();
                        return BadRequest(new ApiResponse(500, ex.Message));
                    }
                }
            
                var newHas = new Has
                {
                    BlogId = newBlog.Id,
                    TagId = tag.Id,
                };
                newData.Add(newHas);
            }

            try
            {
                await _context.Repo<Has>().AddRangeAsync(newData);
                await _context.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _context.DisposeAsync();
                return BadRequest(new ApiResponse(500, ex.Message));
            }

            return Created("Post Blog", _mapper.Map<BlogDTO>(newBlog));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            if(id < 1)
                return BadRequest(new ApiResponse(400));

            var blog = await _context.Repo<Blog>().GetByIdAsync(id);
            if (blog == null)
                return NotFound(new ApiResponse(404));

            try
            {
                _context.Repo<Blog>().Delete(blog);
                await _context.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _context.DisposeAsync();
                return BadRequest(new ApiResponse(500, ex.Message));
            }

            return NoContent();
        }
    }
}
