using CollecthubDotNet.Models;
using CollecthubDotNet.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CollecthubDotNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MobileAppsController : ControllerBase
    {
        private readonly MobileAppService _mobileAppService;

        public MobileAppsController(MobileAppService mobileAppService)
        {
            _mobileAppService = mobileAppService;
        }

        /// <summary>
        /// Create a new mobile app entry
        /// </summary>
        /// <param name="mobileApp">Mobile app data</param>
        /// <returns>Created mobile app</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<MobileApp>>> CreateMobileApp([FromBody] MobileApp mobileApp)
        {
            try
            {
                // Validate platform
                if (!IsValidPlatform(mobileApp.Platform))
                {
                    return BadRequest(new ApiResponse<MobileApp>
                    {
                        Success = false,
                        Message = "Invalid platform. Must be 'Android' or 'iOS'.",
                        Errors = new List<string> { "Platform must be either 'Android' or 'iOS'" }
                    });
                }

                // Clear the ID to ensure MongoDB generates it
                mobileApp.Id = null;
                
                var createdApp = await _mobileAppService.CreateAsync(mobileApp);
                
                return CreatedAtAction(nameof(GetMobileAppById), 
                    new { id = createdApp.Id }, 
                    new ApiResponse<MobileApp>
                    {
                        Success = true,
                        Message = "Mobile app created successfully",
                        Data = createdApp
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<MobileApp>
                {
                    Success = false,
                    Message = "An error occurred while creating the mobile app",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get all mobile apps by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of mobile apps for the user</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<List<MobileApp>>>> GetMobileAppsByUserId([FromRoute] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new ApiResponse<List<MobileApp>>
                    {
                        Success = false,
                        Message = "User ID is required",
                        Errors = new List<string> { "User ID cannot be null or empty" }
                    });
                }

                var mobileApps = await _mobileAppService.GetByUserIdAsync(userId);
                
                return Ok(new ApiResponse<List<MobileApp>>
                {
                    Success = true,
                    Message = $"Found {mobileApps.Count} mobile apps for user",
                    Data = mobileApps
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<MobileApp>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving mobile apps",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get a single mobile app by its ID
        /// </summary>
        /// <param name="id">Mobile app ID</param>
        /// <returns>Mobile app details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MobileApp>>> GetMobileAppById([FromRoute] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new ApiResponse<MobileApp>
                    {
                        Success = false,
                        Message = "Mobile app ID is required",
                        Errors = new List<string> { "ID cannot be null or empty" }
                    });
                }

                var mobileApp = await _mobileAppService.GetByIdAsync(id);
                
                if (mobileApp == null)
                {
                    return NotFound(new ApiResponse<MobileApp>
                    {
                        Success = false,
                        Message = "Mobile app not found",
                        Errors = new List<string> { $"No mobile app found with ID: {id}" }
                    });
                }

                return Ok(new ApiResponse<MobileApp>
                {
                    Success = true,
                    Message = "Mobile app retrieved successfully",
                    Data = mobileApp
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<MobileApp>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the mobile app",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update a mobile app
        /// </summary>
        /// <param name="id">Mobile app ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="updateDto">Update data</param>
        /// <returns>Updated mobile app</returns>
        [HttpPut("{id}/user/{userId}")]
        public async Task<ActionResult<ApiResponse<MobileApp>>> UpdateMobileApp(
            [FromRoute] string id, 
            [FromRoute] string userId, 
            [FromBody] MobileAppUpdateDto updateDto)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new ApiResponse<MobileApp>
                    {
                        Success = false,
                        Message = "Both mobile app ID and user ID are required",
                        Errors = new List<string> { "ID and User ID cannot be null or empty" }
                    });
                }

                // Validate platform if provided
                if (!string.IsNullOrEmpty(updateDto.Platform) && !IsValidPlatform(updateDto.Platform))
                {
                    return BadRequest(new ApiResponse<MobileApp>
                    {
                        Success = false,
                        Message = "Invalid platform. Must be 'Android' or 'iOS'.",
                        Errors = new List<string> { "Platform must be either 'Android' or 'iOS'" }
                    });
                }

                var updatedApp = await _mobileAppService.UpdateAsync(id, userId, updateDto);
                
                if (updatedApp == null)
                {
                    return NotFound(new ApiResponse<MobileApp>
                    {
                        Success = false,
                        Message = "Mobile app not found or you don't have permission to update it",
                        Errors = new List<string> { $"No mobile app found with ID: {id} for user: {userId}" }
                    });
                }

                return Ok(new ApiResponse<MobileApp>
                {
                    Success = true,
                    Message = "Mobile app updated successfully",
                    Data = updatedApp
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<MobileApp>
                {
                    Success = false,
                    Message = "An error occurred while updating the mobile app",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a mobile app
        /// </summary>
        /// <param name="id">Mobile app ID</param>
        /// <returns>Deletion confirmation</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMobileApp([FromRoute] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Mobile app ID is required",
                        Errors = new List<string> { "ID cannot be null or empty" }
                    });
                }

                // First check if the mobile app exists
                var existingApp = await _mobileAppService.GetByIdAsync(id);
                if (existingApp == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Mobile app not found",
                        Errors = new List<string> { $"No mobile app found with ID: {id}" }
                    });
                }

                var deleted = await _mobileAppService.DeleteAsync(id);
                
                if (!deleted)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to delete mobile app",
                        Errors = new List<string> { "Deletion operation failed" }
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Mobile app deleted successfully",
                    Data = new { deletedId = id, deletedAt = DateTime.UtcNow }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the mobile app",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Helper method to validate platform
        /// </summary>
        /// <param name="platform">Platform string</param>
        /// <returns>True if valid platform</returns>
        private static bool IsValidPlatform(string platform)
        {
            return platform.Equals("Android", StringComparison.OrdinalIgnoreCase) || 
                   platform.Equals("iOS", StringComparison.OrdinalIgnoreCase);
        }
    }
}