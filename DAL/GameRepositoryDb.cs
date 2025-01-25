using Domain;
using DTO;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class GameRepositoryDb : IGameRepository
    {
        private readonly AppDbContext _context;

        public GameRepositoryDb(AppDbContext context)
        {
            _context = context;
        }

        public void SaveGame(string jsonStateString, string gameName, string? playerX = null, string? playerO = null)
        {
            var gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(jsonStateString);

            if (gameState == null)
            {
                throw new Exception("Invalid game state.");
            }
            
            var config = _context.Configurations.FirstOrDefault(c =>
                c.BoardWidth == gameState.GameConfiguration.BoardSizeWidth &&
                c.BoardHeight == gameState.GameConfiguration.BoardSizeHeight &&
                c.Name == gameState.GameConfiguration.Name);

            if (config == null)
            {
                config = new Configuration
                {
                    Name = string.IsNullOrWhiteSpace(gameState.GameConfiguration.Name) 
                        ? "Custom" 
                        : gameState.GameConfiguration.Name,
                    BoardWidth = gameState.GameConfiguration.BoardSizeWidth,
                    BoardHeight = gameState.GameConfiguration.BoardSizeHeight,
                };
                _context.Configurations.Add(config);
                _context.SaveChanges();
            }
            
            var existingSaveGame = _context.SaveGames.FirstOrDefault(sg => sg.Name == gameName);
            if (existingSaveGame != null)
            {
                existingSaveGame.State = jsonStateString;
                existingSaveGame.CreatedAtDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                existingSaveGame.PlayerX = playerX;
                existingSaveGame.PlayerO = playerO;
            }
            else
            {
                var saveGame = new SaveGame
                {
                    Name = gameName,
                    CreatedAtDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    State = jsonStateString,
                    ConfigurationId = config.Id,
                    PlayerX = playerX,
                    PlayerO = playerO
                };
                _context.SaveGames.Add(saveGame);
            }

            _context.SaveChanges();
        }

        public bool GameExists(string gameName)
        {
            return _context.SaveGames.Any(sg => sg.Name == gameName);
        }

        public List<string> GetSavedGameNames()
        {
            return _context.SaveGames
                .Select(sg => sg.Name)
                .Distinct()
                .ToList();
        }
        
        public List<string> GetUserGames(string userName)
        {
            return _context.SaveGames
                .Where(sg => sg.PlayerX == userName || sg.PlayerO == userName)
                .Select(sg => sg.Name)
                .Distinct()
                .ToList();
        }

        public (string? jsonStateString, string? playerX, string? playerO) LoadGame(string gameName)
        {
            var saveGame = _context.SaveGames
                .Include(sg => sg.Configuration) // selle võib vist tehniliselt eemaldada, sest mäng saab konfi kätte ka JSONist
                .FirstOrDefault(sg => sg.Name == gameName);

            if (saveGame == null)
            {
                return (null, null, null);
            }

            return (saveGame.State, saveGame.PlayerX, saveGame.PlayerO);
        }

        public void DeleteGame(string gameName)
        {
            var saveGames = _context.SaveGames
                .Where(sg => sg.Name == gameName)
                .ToList();

            if (saveGames.Any())
            {
                _context.SaveGames.RemoveRange(saveGames);
                _context.SaveChanges();
            }

            // Optionally, remove the Configuration if no more SaveGames are linked to it
            var configsToDelete = _context.Configurations
                .Where(c => !_context.SaveGames.Any(sg => sg.ConfigurationId == c.Id))
                .ToList();

            if (configsToDelete.Any())
            {
                _context.Configurations.RemoveRange(configsToDelete);
                _context.SaveChanges();
            }
        }
    }
}
