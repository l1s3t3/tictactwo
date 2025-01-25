using System.Text.Json;
using DTO;

namespace DAL
{
    public class GameRepositoryJson : IGameRepository
    {
        public void SaveGame(string jsonStateString, string gameName, string? playerX = null, string? playerO = null)
        {
            var filename = FileHelper.BasePath
                           + gameName
                           + FileHelper.GameExtension;

            var saveGameData = new AllGameDataJson
            {
                JsonStateString = jsonStateString,
                PlayerX = playerX,
                PlayerO = playerO
            };

            var json = JsonSerializer.Serialize(saveGameData);
            File.WriteAllText(filename, json);
        }

        
        public bool GameExists(string gameName)
        {
            var filename = Path.Combine(FileHelper.BasePath, gameName + FileHelper.GameExtension);
            return File.Exists(filename);
        }

        public List<string> GetSavedGameNames()
        {
            if (!Directory.Exists(FileHelper.BasePath))
            {
                Directory.CreateDirectory(FileHelper.BasePath);
            }

            return Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension)
                .Select(fullFileName =>
                {
                    var fileName = Path.GetFileName(fullFileName);
                    while (Path.HasExtension(fileName))
                    {
                        fileName = Path.GetFileNameWithoutExtension(fileName);
                    }
                    return fileName;
                })
                .ToList();
        }
        
        public List<string> GetUserGames(string userName)
        {
            var games = new List<string>();
            if (!Directory.Exists(FileHelper.BasePath))
            {
                Directory.CreateDirectory(FileHelper.BasePath);
            }
            
            foreach (var gameFile in Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension))
            {
                var saveGameData = JsonSerializer.Deserialize<AllGameDataJson>(File.ReadAllText(gameFile));
                if (saveGameData != null && (saveGameData.PlayerX == userName || saveGameData.PlayerO == userName))
                {
                    var gameName = Path.GetFileNameWithoutExtension(gameFile);
                    var gameNameWithoutEx = gameName.Replace(".game", "");
                    games.Add(gameNameWithoutEx);
                }
            }

            return games;
        }

        public (string? jsonStateString, string? playerX, string? playerO) LoadGame(string gameName)
        {
            gameName = gameName.Replace(".game", "").ToLower();
            var filename = Path.Combine(FileHelper.BasePath, gameName + FileHelper.GameExtension);

            if (!File.Exists(filename))
            {
                Console.WriteLine($"File '{filename}' does not exist.");
                return (null, null, null);
            }

            try
            {
                var json = File.ReadAllText(filename);
                var saveGameData = JsonSerializer.Deserialize<AllGameDataJson>(json);
                return (saveGameData?.JsonStateString, saveGameData?.PlayerX, saveGameData?.PlayerO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading the saved game file: {ex.Message}");
                return (null, null, null);
            }
        }
        
        public void DeleteGame(string gameName)
        {
            gameName = gameName.Replace(".game", "").ToLower();
            var filename = Path.Combine(FileHelper.BasePath, gameName + FileHelper.GameExtension);

            // Verify the directory exists before attempting to delete
            if (!Directory.Exists(FileHelper.BasePath))
            {
                Console.WriteLine("Save directory does not exist. No games to delete.");
                return;
            }
            
            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                    Console.WriteLine($"Deleted saved game '{filename}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file '{filename}': {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"File '{filename}' does not exist.");
            }
        }

    }
}