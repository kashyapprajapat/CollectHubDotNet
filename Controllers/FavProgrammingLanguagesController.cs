using Microsoft.AspNetCore.Mvc;
using CollecthubDotNet.Models;
using CollecthubDotNet.Services;

namespace CollecthubDotNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavProgrammingLanguagesController : ControllerBase
    {
        private readonly FavProgrammingLanguageService _favProgrammingLanguageService;

        public FavProgrammingLanguagesController(FavProgrammingLanguageService favProgrammingLanguageService)
        {
            _favProgrammingLanguageService = favProgrammingLanguageService;
        }

        // POST: api/FavProgrammingLanguages
        [HttpPost]
        public async Task<ActionResult<ApiResponse<FavProgrammingLanguage>>> CreateFavProgrammingLanguage([FromBody] FavProgrammingLanguage favProgrammingLanguage)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(favProgrammingLanguage.UserId) ||
                    string.IsNullOrWhiteSpace(favProgrammingLanguage.ProgrammingLanguageName) ||
                    string.IsNullOrWhiteSpace(favProgrammingLanguage.UseCase) ||
                    string.IsNullOrWhiteSpace(favProgrammingLanguage.Reason))
                {
                    return BadRequest(new ApiResponse<FavProgrammingLanguage>
                    {
                        Success = false,
                        Message = "All fields (UserId, ProgrammingLanguageName, UseCase, Reason) are required.",
                        Errors = new List<string> { "Missing required fields" }
                    });
                }

                var createdFavLanguage = await _favProgrammingLanguageService.CreateAsync(favProgrammingLanguage);

                return CreatedAtAction(
                    nameof(GetFavProgrammingLanguagesByUserId),
                    new { userId = createdFavLanguage.UserId },
                    new ApiResponse<FavProgrammingLanguage>
                    {
                        Success = true,
                        Message = "Favorite programming language created successfully.",
                        Data = createdFavLanguage
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<FavProgrammingLanguage>
                {
                    Success = false,
                    Message = "An error occurred while creating the favorite programming language.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // GET: api/FavProgrammingLanguages?userId=xxx
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<FavProgrammingLanguage>>>> GetFavProgrammingLanguagesByUserId([FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new ApiResponse<List<FavProgrammingLanguage>>
                    {
                        Success = false,
                        Message = "UserId parameter is required.",
                        Errors = new List<string> { "Missing userId parameter" }
                    });
                }

                var favLanguages = await _favProgrammingLanguageService.GetByUserIdAsync(userId);

                return Ok(new ApiResponse<List<FavProgrammingLanguage>>
                {
                    Success = true,
                    Message = favLanguages.Count > 0 ? 
                        $"Found {favLanguages.Count} favorite programming language(s)." : 
                        "No favorite programming languages found for this user.",
                    Data = favLanguages
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<FavProgrammingLanguage>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving favorite programming languages.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // PUT: api/FavProgrammingLanguages?id=xxx&userId=xxx
        [HttpPut]
        public async Task<ActionResult<ApiResponse<FavProgrammingLanguage>>> UpdateFavProgrammingLanguage(
            [FromQuery] string id, 
            [FromQuery] string userId, 
            [FromBody] FavProgrammingLanguage updatedFavLanguage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new ApiResponse<FavProgrammingLanguage>
                    {
                        Success = false,
                        Message = "Both id and userId parameters are required.",
                        Errors = new List<string> { "Missing required parameters" }
                    });
                }

                // Check if the record exists
                var existingRecord = await _favProgrammingLanguageService.GetByIdAsync(id);
                if (existingRecord == null)
                {
                    return NotFound(new ApiResponse<FavProgrammingLanguage>
                    {
                        Success = false,
                        Message = "Favorite programming language not found.",
                        Errors = new List<string> { "Record not found" }
                    });
                }

                // Check if the record belongs to the user
                if (existingRecord.UserId != userId)
                {
                    return Forbid();
                }

                // Validate required fields in the update
                if (string.IsNullOrWhiteSpace(updatedFavLanguage.ProgrammingLanguageName) ||
                    string.IsNullOrWhiteSpace(updatedFavLanguage.UseCase) ||
                    string.IsNullOrWhiteSpace(updatedFavLanguage.Reason))
                {
                    return BadRequest(new ApiResponse<FavProgrammingLanguage>
                    {
                        Success = false,
                        Message = "All fields (ProgrammingLanguageName, UseCase, Reason) are required for update.",
                        Errors = new List<string> { "Missing required fields" }
                    });
                }

                var success = await _favProgrammingLanguageService.UpdateAsync(id, updatedFavLanguage);

                if (success)
                {
                    // Get the updated record
                    var updatedRecord = await _favProgrammingLanguageService.GetByIdAsync(id);
                    return Ok(new ApiResponse<FavProgrammingLanguage>
                    {
                        Success = true,
                        Message = "Favorite programming language updated successfully.",
                        Data = updatedRecord
                    });
                }
                else
                {
                    return StatusCode(500, new ApiResponse<FavProgrammingLanguage>
                    {
                        Success = false,
                        Message = "Failed to update favorite programming language.",
                        Errors = new List<string> { "Update operation failed" }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<FavProgrammingLanguage>
                {
                    Success = false,
                    Message = "An error occurred while updating the favorite programming language.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // DELETE: api/FavProgrammingLanguages?id=xxx
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<object>>> DeleteFavProgrammingLanguage([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Id parameter is required.",
                        Errors = new List<string> { "Missing id parameter" }
                    });
                }

                // Check if the record exists before deletion
                var exists = await _favProgrammingLanguageService.ExistsAsync(id);
                if (!exists)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Favorite programming language not found.",
                        Errors = new List<string> { "Record not found" }
                    });
                }

                var success = await _favProgrammingLanguageService.DeleteAsync(id);

                if (success)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Favorite programming language deleted successfully.",
                        Data = new { deletedId = id }
                    });
                }
                else
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to delete favorite programming language.",
                        Errors = new List<string> { "Delete operation failed" }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the favorite programming language.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}