using Microsoft.AspNetCore.Mvc;
using CollecthubDotNet.Models;
using CollecthubDotNet.Services;
using System.ComponentModel.DataAnnotations;

namespace CollecthubDotNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GameService _gameService;

        public GamesController(GameService gameService)
        {
            _gameService = gameService;
        }

        // POST: api/games
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Game>>> CreateGame([FromBody] CreateGameDto gameDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<Game>
                    {
                        Success = false,
                        Message = "Invalid data provided",
                        Data = null,
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                    });
                }

                var game = await _gameService.CreateGameAsync(gameDto);

                return CreatedAtAction(
                    nameof(GetGamesByUserId),
                    new { userId = game.UserId },
                    new ApiResponse<Game>
                    {
                        Success = true,
                        Message = "Game created successfully",
                        Data = game
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Game>
                {
                    Success = false,
                    Message = "An error occurred while creating the game",
                    Data = null,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // GET: api/games/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<List<Game>>>> GetGamesByUserId([Required] string userId)
        {
            try
            {
                var games = await _gameService.GetGamesByUserIdAsync(userId);

                return Ok(new ApiResponse<List<Game>>
                {
                    Success = true,
                    Message = games.Any() ? "Games retrieved successfully" : "No games found for this user",
                    Data = games
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<Game>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving games",
                    Data = null,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // GET: api/games/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Game>>> GetGameById([Required] string id)
        {
            try
            {
                var game = await _gameService.GetGameByIdAsync(id);

                if (game == null)
                {
                    return NotFound(new ApiResponse<Game>
                    {
                        Success = false,
                        Message = "Game not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<Game>
                {
                    Success = true,
                    Message = "Game retrieved successfully",
                    Data = game
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Game>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the game",
                    Data = null,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // PUT: api/games/{id}/user/{userId}
        [HttpPut("{id}/user/{userId}")]
        public async Task<ActionResult<ApiResponse<Game>>> UpdateGame(
            [Required] string id, 
            [Required] string userId, 
            [FromBody] UpdateGameDto updateGameDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<Game>
                    {
                        Success = false,
                        Message = "Invalid data provided",
                        Data = null,
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                    });
                }

                // Check if the game exists and belongs to the user
                var gameExists = await _gameService.GameExistsForUserAsync(id, userId);
                if (!gameExists)
                {
                    return NotFound(new ApiResponse<Game>
                    {
                        Success = false,
                        Message = "Game not found or you don't have permission to update it",
                        Data = null
                    });
                }

                var updatedGame = await _gameService.UpdateGameAsync(id, userId, updateGameDto);

                if (updatedGame == null)
                {
                    return NotFound(new ApiResponse<Game>
                    {
                        Success = false,
                        Message = "Game not found or update failed",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<Game>
                {
                    Success = true,
                    Message = "Game updated successfully",
                    Data = updatedGame
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Game>
                {
                    Success = false,
                    Message = "An error occurred while updating the game",
                    Data = null,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // DELETE: api/games/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteGame([Required] string id)
        {
            try
            {
                var deleted = await _gameService.DeleteGameAsync(id);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Game not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Game deleted successfully",
                    Data = new { deletedId = id }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the game",
                    Data = null,
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}