using CollecthubDotNet.Models;
using CollecthubDotNet.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CollecthubDotNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavVehicleController : ControllerBase
    {
        private readonly FavVehicleService _favVehicleService;

        public FavVehicleController(FavVehicleService favVehicleService)
        {
            _favVehicleService = favVehicleService;
        }

        /// <summary>
        /// Get all favorite vehicles for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of favorite vehicles</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<FavVehicle>>>> GetVehiclesByUserId([FromQuery, Required] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new ApiResponse<List<FavVehicle>>
                    {
                        Success = false,
                        Message = "User ID is required",
                        Errors = new List<string> { "User ID parameter cannot be empty" }
                    });
                }

                var vehicles = await _favVehicleService.GetByUserIdAsync(userId);
                
                return Ok(new ApiResponse<List<FavVehicle>>
                {
                    Success = true,
                    Message = vehicles.Count > 0 ? $"Found {vehicles.Count} vehicles" : "No vehicles found for this user",
                    Data = vehicles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<FavVehicle>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving vehicles",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create a new favorite vehicle
        /// </summary>
        /// <param name="vehicle">Vehicle data</param>
        /// <returns>Created vehicle</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<FavVehicle>>> CreateVehicle([FromBody] FavVehicle vehicle)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new ApiResponse<FavVehicle>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                var createdVehicle = await _favVehicleService.CreateAsync(vehicle);
                
                return CreatedAtAction(
                    nameof(GetVehicleById), 
                    new { id = createdVehicle.Id }, 
                    new ApiResponse<FavVehicle>
                    {
                        Success = true,
                        Message = "Vehicle created successfully",
                        Data = createdVehicle
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<FavVehicle>
                {
                    Success = false,
                    Message = "An error occurred while creating the vehicle",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get a specific vehicle by ID
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <returns>Vehicle details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<FavVehicle>>> GetVehicleById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new ApiResponse<FavVehicle>
                    {
                        Success = false,
                        Message = "Vehicle ID is required",
                        Errors = new List<string> { "ID parameter cannot be empty" }
                    });
                }

                var vehicle = await _favVehicleService.GetByIdAsync(id);
                
                if (vehicle == null)
                {
                    return NotFound(new ApiResponse<FavVehicle>
                    {
                        Success = false,
                        Message = "Vehicle not found",
                        Errors = new List<string> { $"No vehicle found with ID: {id}" }
                    });
                }

                return Ok(new ApiResponse<FavVehicle>
                {
                    Success = true,
                    Message = "Vehicle retrieved successfully",
                    Data = vehicle
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<FavVehicle>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the vehicle",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update an existing vehicle
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="vehicle">Updated vehicle data</param>
        /// <returns>Update result</returns>
        [HttpPut]
        public async Task<ActionResult<ApiResponse<object>>> UpdateVehicle(
            [FromQuery, Required] string id, 
            [FromQuery, Required] string userId, 
            [FromBody] FavVehicle vehicle)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Both ID and User ID are required",
                        Errors = new List<string> { "ID and userId parameters cannot be empty" }
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                // Check if vehicle exists
                var existingVehicle = await _favVehicleService.GetByIdAsync(id);
                if (existingVehicle == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Vehicle not found",
                        Errors = new List<string> { $"No vehicle found with ID: {id}" }
                    });
                }

                // Check if the vehicle belongs to the user
                if (existingVehicle.UserId != userId)
                {
                    return Forbid();
                }

                // Preserve the original user ID
                vehicle.UserId = userId;
                var updated = await _favVehicleService.UpdateAsync(id, vehicle);
                
                if (!updated)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to update vehicle",
                        Errors = new List<string> { "Update operation did not modify any documents" }
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Vehicle updated successfully",
                    Data = new { Id = id, UserId = userId }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the vehicle",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a vehicle
        /// </summary>
        /// <param name="id">Vehicle ID to delete</param>
        /// <returns>Delete result</returns>
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<object>>> DeleteVehicle([FromQuery, Required] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Vehicle ID is required",
                        Errors = new List<string> { "ID parameter cannot be empty" }
                    });
                }

                // Check if vehicle exists before attempting to delete
                var exists = await _favVehicleService.ExistsAsync(id);
                if (!exists)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Vehicle not found",
                        Errors = new List<string> { $"No vehicle found with ID: {id}" }
                    });
                }

                var deleted = await _favVehicleService.DeleteAsync(id);
                
                if (!deleted)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to delete vehicle",
                        Errors = new List<string> { "Delete operation did not remove any documents" }
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Vehicle deleted successfully",
                    Data = new { DeletedId = id }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the vehicle",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}