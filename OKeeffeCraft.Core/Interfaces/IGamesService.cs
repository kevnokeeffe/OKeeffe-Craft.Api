using OKeeffeCraft.Models;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface IGamesService
    {
        Task<ServiceResponse<string>> CreateSnakeHighScore(CreateSnakeHighScoreModel snakeHighScore);
        Task<ServiceResponse<IEnumerable<SnakeHighScoreModel>>> GetSnakeHighScores();
        Task<ServiceResponse<SnakeHighScoreModel>> GetSnakeHighScoreById(string id);
        Task<ServiceResponse<string>> UpdateSnakeHighScore(string id, UpdateSnakeHighScoreModel updatedSnakeHighScore);
    }
}
