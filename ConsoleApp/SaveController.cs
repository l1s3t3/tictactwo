using DTO;
using GameBrain;

namespace ConsoleApp;

public class SaveController
{
    private readonly GameService _gameService;

    public SaveController(GameService gameService)
    {
        _gameService = gameService;
    }

    public void SaveGame()
    {
        Console.WriteLine("Enter a cool name for the saved game:");
        string? saveName = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(saveName))
        {
            Console.WriteLine("Save name cannot be empty! Please enter a valid name:");
            saveName = Console.ReadLine();
        }

        if (_gameService.GameExists(saveName))
        {
            Console.WriteLine($"A saved game with the name '{saveName}' already exists.");
            Console.WriteLine("Do you want to overwrite it? (Y/N)");
            string? overwrite = Console.ReadLine();
            if (!string.Equals(overwrite, "Y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Save canceled.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }
        }

        _gameService.SaveGame(saveName);
        Console.WriteLine($"Game saved as '{saveName}'.");
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    public GameState? SelectSavedGame()
        {
            var savedGameFiles = _gameService.GetSavedGameNames();
            if (savedGameFiles.Count == 0)
            {
                Console.WriteLine("No saved games available.");
                Console.WriteLine("Press Enter to return.");
                Console.ReadLine();
                return null;
            }

            Console.WriteLine("Saved Games:");
            for (int i = 0; i < savedGameFiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {savedGameFiles[i]}");
            }
            Console.WriteLine($"{savedGameFiles.Count + 1}) Delete a saved game");
            Console.WriteLine($"{savedGameFiles.Count + 2}) Return");

            int choice = ControllerHelpers.AskNumberInput($"Choose a saved game to load (1-{savedGameFiles.Count + 2}): ", 1, savedGameFiles.Count + 2);

            if (choice == savedGameFiles.Count + 1)
            {
                DeleteSavedGame();
                return SelectSavedGame();
            }
            else if (choice == savedGameFiles.Count + 2)
            {
                return null; // Tagasi mängu konfiguratsiooni menüüsse
            }
            else
            {
                var gameName = savedGameFiles[choice - 1];
                try
                {
                    var (gameState, playerX, playerO) = _gameService.LoadGame(gameName);
                    Console.WriteLine($"Loaded game for Player X: {playerX}, Player O: {playerO}");
                    return gameState;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading the saved game: {ex.Message}");
                    return null;
                }
            }
        }

        private void DeleteSavedGame()
        {
            var savedGameFiles = _gameService.GetSavedGameNames();
            if (savedGameFiles.Count == 0)
            {
                Console.WriteLine("No saved games available to delete.");
                Console.WriteLine("Press Enter to return.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Select a saved game to delete:");
            for (int i = 0; i < savedGameFiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {savedGameFiles[i]}");
            }
            Console.WriteLine($"{savedGameFiles.Count + 1}) Cancel");

            int choice = ControllerHelpers.AskNumberInput($"Choose a saved game to delete (1-{savedGameFiles.Count + 1}): ", 1, savedGameFiles.Count + 1);

            if (choice == savedGameFiles.Count + 1)
            {
                return;
            }
            
            var gameName = savedGameFiles[choice - 1];
            Console.WriteLine($"Are you sure you want to delete '{gameName}'? (Y/N)");
            string? confirm = Console.ReadLine();

            if (confirm?.Trim().Equals("Y", StringComparison.OrdinalIgnoreCase) == true)
            {
                try
                {
                    _gameService.DeleteGame(gameName);
                    Console.WriteLine($"Deleted saved game '{gameName}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting saved game '{gameName}': {ex.Message}");
                }

                Console.WriteLine("Press Enter to continue.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Deletion canceled.");
                Console.WriteLine("Press Enter to continue.");
                Console.ReadLine();
            }
        }
}