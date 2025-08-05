using CollecthubDotNet.Models;
using CollecthubDotNet.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CollecthubDotNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YouTubeChannelsController : ControllerBase
    {
        private readonly YouTubeChannelService _youtubeChannelService;

        public YouTubeChannelsController(YouTubeChannelService youtubeChannelService)
        {
            _youtubeChannelService = youtubeChannelService;
        }

        /// <summary>
        /// Create a new YouTube channel
        /// </summary>
        /// <param name="request">YouTube channel creation data</param>
        /// <returns>Created YouTube channel</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<YouTubeChannel>>> CreateYouTubeChannel([FromBody] CreateYouTubeChannelRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new ApiResponse<YouTubeChannel>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                var youtubeChannel = await _youtubeChannelService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetYouTubeChannelById),
                    new { id = youtubeChannel.Id },
                    new ApiResponse<YouTubeChannel>
                    {
                        Success = true,
                        Message = "YouTube channel created successfully",
                        Data = youtubeChannel
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<YouTubeChannel>
                {
                    Success = false,
                    Message = "An error occurred while creating the YouTube channel",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get all YouTube channels by user ID
        /// </summary>
        /// <param name="userId">User ID to filter channels</param>
        /// <returns>List of YouTube channels for the user</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<YouTubeChannel>>>> GetYouTubeChannelsByUserId([FromQuery, Required] string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new ApiResponse<List<YouTubeChannel>>
                    {
                        Success = false,
                        Message = "User ID is required",
                        Errors = new List<string> { "UserId parameter cannot be empty" }
                    });
                }

                var youtubeChannels = await _youtubeChannelService.GetByUserIdAsync(userId);

                return Ok(new ApiResponse<List<YouTubeChannel>>
                {
                    Success = true,
                    Message = youtubeChannels.Count > 0 ? "YouTube channels retrieved successfully" : "No YouTube channels found for this user",
                    Data = youtubeChannels
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<YouTubeChannel>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving YouTube channels",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get a specific YouTube channel by ID
        /// </summary>
        /// <param name="id">YouTube channel ID</param>
        /// <returns>YouTube channel details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<YouTubeChannel>>> GetYouTubeChannelById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new ApiResponse<YouTubeChannel>
                    {
                        Success = false,
                        Message = "Channel ID is required",
                        Errors = new List<string> { "ID parameter cannot be empty" }
                    });
                }

                var youtubeChannel = await _youtubeChannelService.GetByIdAsync(id);

                if (youtubeChannel == null)
                {
                    return NotFound(new ApiResponse<YouTubeChannel>
                    {
                        Success = false,
                        Message = "YouTube channel not found"
                    });
                }

                return Ok(new ApiResponse<YouTubeChannel>
                {
                    Success = true,
                    Message = "YouTube channel retrieved successfully",
                    Data = youtubeChannel
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<YouTubeChannel>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the YouTube channel",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update a YouTube channel
        /// </summary>
        /// <param name="id">YouTube channel ID</param>
        /// <param name="userId">User ID (owner verification)</param>
        /// <param name="request">Update data</param>
        /// <returns>Updated YouTube channel</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<YouTubeChannel>>> UpdateYouTubeChannel(
            string id, 
            [FromQuery, Required] string userId, 
            [FromBody] UpdateYouTubeChannelRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new ApiResponse<YouTubeChannel>
                    {
                        Success = false,
                        Message = "Channel ID is required",
                        Errors = new List<string> { "ID parameter cannot be empty" }
                    });
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new ApiResponse<YouTubeChannel>
                    {
                        Success = false,
                        Message = "User ID is required",
                        Errors = new List<string> { "UserId parameter cannot be empty" }
                    });
                }

                var updatedYouTubeChannel = await _youtubeChannelService.UpdateAsync(id, userId, request);

                if (updatedYouTubeChannel == null)
                {
                    return NotFound(new ApiResponse<YouTubeChannel>
                    {
                        Success = false,
                        Message = "YouTube channel not found or you don't have permission to update it"
                    });
                }

                return Ok(new ApiResponse<YouTubeChannel>
                {
                    Success = true,
                    Message = "YouTube channel updated successfully",
                    Data = updatedYouTubeChannel
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<YouTubeChannel>
                {
                    Success = false,
                    Message = "An error occurred while updating the YouTube channel",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a YouTube channel
        /// </summary>
        /// <param name="id">YouTube channel ID</param>
        /// <returns>Deletion confirmation</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteYouTubeChannel(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Channel ID is required",
                        Errors = new List<string> { "ID parameter cannot be empty" }
                    });
                }

                var deleted = await _youtubeChannelService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "YouTube channel not found"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "YouTube channel deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the YouTube channel",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}