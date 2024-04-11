using Microsoft.AspNetCore.Mvc;
using OKeeffeCraft.Authorization;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace OKeeffeCraft.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GamesController : BaseController
    {

        private readonly IGamesService _gamesService;

        public GamesController(IGamesService gamesService)
        {
            _gamesService = gamesService;
        }

        [AllowAnonymous]
        [HttpPost("snake-high-score")]
        [SwaggerOperation(Summary = "Create a new snake high score", Description = "Create a new snake high score")]
        [SwaggerResponse(200, "The snake high score was created successfully")]
        [SwaggerResponse(400, "The snake high score could not be created")]
        public async Task<IActionResult> CreateSnakeHighScore(CreateSnakeHighScoreModel model)
        {
            var response = await _gamesService.CreateSnakeHighScore(model);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("snake-high-scores")]
        [SwaggerOperation(Summary = "Get all snake high scores", Description = "Get all snake high scores")]
        [SwaggerResponse(200, "The snake high scores were retrieved successfully")]
        [SwaggerResponse(400, "The snake high scores could not be retrieved")]
        public async Task<IActionResult> GetSnakeHighScores()
        {
            var response = await _gamesService.GetSnakeHighScores();
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("snake-high-score/{id}")]
        [SwaggerOperation(Summary = "Get a snake high score by ID", Description = "Get a snake high score by ID")]
        [SwaggerResponse(200, "The snake high score was retrieved successfully")]
        [SwaggerResponse(400, "The snake high score could not be retrieved")]
        public async Task<IActionResult> GetSnakeHighScoreById(string id)
        {
            var response = await _gamesService.GetSnakeHighScoreById(id);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPut("snake-high-score/{id}")]
        [SwaggerOperation(Summary = "Update a snake high score", Description = "Update a snake high score")]
        [SwaggerResponse(200, "The snake high score was updated successfully")]
        [SwaggerResponse(400, "The snake high score could not be updated")]
        public async Task<IActionResult> UpdateSnakeHighScore(string id, UpdateSnakeHighScoreModel updatedSnakeHighScore)
        {
            var response = await _gamesService.UpdateSnakeHighScore(id, updatedSnakeHighScore);
            return Ok(response);
        }

    }
}
