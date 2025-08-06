using CollecthubDotNet.Models;
using CollecthubDotNet.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollecthubDotNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavMusicController : ControllerBase
    {
        private readonly FavMusicService _favMusicService;

        public FavMusicController(FavMusicService favMusicService)
        {
            _favMusicService = favMusicService;
        }

        // GET: api/FavMusic?userId={userId}
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<FavMusic>>>> GetFavMusicByUserId([FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new ApiResponse<List<FavMusic>>
                    {
                        Success = false,
                        Message = "UserId is required",
                        Errors = new List<string> { "UserId parameter cannot be empty" }
                    });
                }

                var favMusicList = await _favMusicService.GetByUserIdAsync(userId);

                return Ok(new ApiResponse<List<FavMusic>>
                {
                    Success = true,
                    Message = $"Found {favMusicList.Count} favorite music records for user {userId}",
                    Data = favMusicList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<FavMusic>>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // POST: api/FavMusic
        [HttpPost]
        public async Task<ActionResult<ApiResponse<FavMusic>>> CreateFavMusic([FromBody] FavMusic favMusic)
        {
            try
            {
                if (favMusic == null)
                {
                    return BadRequest(new ApiResponse<FavMusic>
                    {
                        Success = false,
                        Message = "FavMusic data is required",
                        Errors = new List<string> { "Request body cannot be empty" }
                    });
                }

                if (string.IsNullOrEmpty(favMusic.UserId) || 
                    string.IsNullOrEmpty(favMusic.MusicName) || 
                    string.IsNullOrEmpty(favMusic.Singer))
                {
                    return BadRequest(new ApiResponse<FavMusic>
                    {
                        Success = false,
                        Message = "Required fields are missing",
                        Errors = new List<string> { "UserId, MusicName, and Singer are required fields" }
                    });
                }

                var createdFavMusic = await _favMusicService.CreateAsync(favMusic);

                return CreatedAtAction(
                    nameof(GetFavMusicByUserId),
                    new { userId = createdFavMusic.UserId },
                    new ApiResponse<FavMusic>
                    {
                        Success = true,
                        Message = "Favorite music created successfully",
                        Data = createdFavMusic
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<FavMusic>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // PUT: api/FavMusic/{id}?userId={userId}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateFavMusic(string id, [FromQuery] string userId, [FromBody] FavMusic updatedFavMusic)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "ID and UserId are required",
                        Errors = new List<string> { "Both id parameter and userId query parameter are required" }
                    });
                }

                if (updatedFavMusic == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Update data is required",
                        Errors = new List<string> { "Request body cannot be empty" }
                    });
                }

                if (string.IsNullOrEmpty(updatedFavMusic.MusicName) || 
                    string.IsNullOrEmpty(updatedFavMusic.Singer))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Required fields are missing",
                        Errors = new List<string> { "MusicName and Singer are required fields" }
                    });
                }

                var isUpdated = await _favMusicService.UpdateAsync(id, userId, updatedFavMusic);

                if (!isUpdated)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Favorite music not found or not authorized to update",
                        Errors = new List<string> { $"No favorite music found with id {id} for user {userId}" }
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Favorite music updated successfully",
                    Data = new { Id = id, UserId = userId }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // DELETE: api/FavMusic/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteFavMusic(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "ID is required",
                        Errors = new List<string> { "Id parameter cannot be empty" }
                    });
                }

                var isDeleted = await _favMusicService.DeleteAsync(id);

                if (!isDeleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Favorite music not found",
                        Errors = new List<string> { $"No favorite music found with id {id}" }
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Favorite music deleted successfully",
                    Data = new { Id = id }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}