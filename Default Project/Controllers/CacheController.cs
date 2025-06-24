using Default_Project.Cores.Interfaces;
using Default_Project.Cores;
using Default_Project.Errors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Default_Project.Controllers
{
    public class CacheController : ApiBaseController
    {
        private readonly ICache _cacheService;

        public CacheController(ICache _cacheService)
        {
            this._cacheService = _cacheService;
        }

        public record AR(int id, string link);

        [HttpPost]
        public async Task<IActionResult> SetCache([FromQuery] string key, [FromQuery] string value, [FromQuery] int expireSeconds)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value) || expireSeconds <= 0)
                return BadRequest(new ApiResponse(400, "Invalid key, value, or expire time."));

            await _cacheService.CacheDataAsync(key, value, TimeSpan.FromSeconds(expireSeconds));
            return Ok($"Cached key '{key}' with expiration {expireSeconds} seconds.");
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetCache([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest(new ApiResponse(400, "Key is required."));

            var data = await _cacheService.GetDataAsync(key);
            if (data == null)
                return NotFound(new ApiResponse(404, $"No data found for key '{key}'."));

            return Ok(data);
        }
    }
}
