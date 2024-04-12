using AutoMapper;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Entities;
using OKeeffeCraft.Models;

namespace OKeeffeCraft.Core.Services
{
    public class GamesService : IGamesService
    {
        private readonly IMongoDBService _context;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;

        public GamesService(
            IMongoDBService context,
            IMapper mapper,
            ILogService logService)
        {
            _context = context;
            _mapper = mapper;
            _logService = logService;
        }

        public async Task<ServiceResponse<string>> CreateSnakeHighScore(CreateSnakeHighScoreModel snakeHighScore)
        {
            try
            {
                if (snakeHighScore == null)
                {
                    return new ServiceResponse<string> { Data = null, Message = "Invalid score", Success = false };
                }
                SnakeHighScore model = _mapper.Map<SnakeHighScore>(snakeHighScore);
                model.CreatedDate = DateTime.UtcNow;
                await _context.CreateSnakeHighScoreAsync(model);
                return new ServiceResponse<string> { Data = null, Message = "High score created", Success = true };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, null, null);
                return new ServiceResponse<string> { Data = null, Message = "Error creating score", Success = false };
            }
        }

        public async Task<ServiceResponse<IEnumerable<SnakeHighScoreModel>>> GetSnakeHighScores()
        {
            try
            {
                var scores = await _context.GetSnakeHighScoresAsync();
                var mappedScores = _mapper.Map<IEnumerable<SnakeHighScoreModel>>(scores);
                return new ServiceResponse<IEnumerable<SnakeHighScoreModel>> { Data = mappedScores, Message = "Scores retrieved successfully", Success = true };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, null, null);
                return new ServiceResponse<IEnumerable<SnakeHighScoreModel>> { Data = null, Message = "Error retrieving scores", Success = false };
            }

        }

        public async Task<ServiceResponse<SnakeHighScoreModel>> GetSnakeHighScoreById(string id)
        {
            try
            {
                var score = await _context.GetSnakeHighScoreAsync(id);
                var mappedScore = _mapper.Map<SnakeHighScoreModel>(score);
                return new ServiceResponse<SnakeHighScoreModel> { Data = mappedScore, Message = "Score retrieved successfully", Success = true };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, null, null);
                return new ServiceResponse<SnakeHighScoreModel> { Data = null, Message = "Error retrieving score", Success = false };
            }

        }

        public async Task<ServiceResponse<string>> UpdateSnakeHighScore(string id, UpdateSnakeHighScoreModel model)
        {
            try
            {
                if (model == null)
                {
                    return new ServiceResponse<string> { Data = null, Message = "Invalid score", Success = false };
                }
                var highScore = await _context.GetSnakeHighScoreAsync(id);
                var updatedSnakeHighScore = _mapper.Map(model, highScore);
                updatedSnakeHighScore.UpdatedDate = DateTime.UtcNow;
                await _context.UpdateSnakeHighScoreAsync(id, updatedSnakeHighScore);
                return new ServiceResponse<string> { Data = null, Message = "Score updated", Success = true };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, null, null);
                return new ServiceResponse<string> { Data = null, Message = "Error updating score", Success = false };
            }
        }
    }
}
