using DTO;
using GameBrain;

namespace ConsoleApp
{
    public class ConfController
    {
        private readonly GameService _gameService;
        private readonly SaveController _saveController;

        public ConfController(GameService gameService, SaveController saveController)
        {
            _gameService = gameService;
            _saveController = saveController;
        }

        public GameConfiguration SelectGameConfiguration()
        {
            Console.WriteLine("Select Game Configuration:");
            Console.WriteLine("1) Classical");
            Console.WriteLine("2) Customize Game");
            Console.WriteLine("3) Saved Games");

            int choice = ControllerHelpers.AskNumberInput("Choose an option (1, 2 or 3): ", 1, 3);

            GameConfiguration config;

            if (choice == 1)
            {
                config = _gameService.GetConfigurationByName("Classical");
                _gameService.InitializeGame(config);
            }
            else if (choice == 2)
            {
                config = CreateGameConfiguration();
                _gameService.InitializeGame(config);
            }
            else
            {
                var gameState = _saveController.SelectSavedGame();
                if (gameState == null) // see on null, sest salvestatud mängu ei valitud ja minnakse tagasi konfiguratsiooni valima
                {
                    return SelectGameConfiguration();
                }
                config = gameState.GameConfiguration;
                _gameService.InitializeGame(config, gameState);
            }

            Console.WriteLine($"Number of pieces per player will be {config.NumberOfPieces}.");

            return config;
        }

        private GameConfiguration CreateGameConfiguration()
        {
            Console.WriteLine("Create your game configuration:");

            Console.Write("Enter board width (min 5, max 30): ");
            int boardWidth = ControllerHelpers.AskNumberInput(min: 5, max: 30);

            Console.Write("Enter board height (min 5, max 30): ");
            int boardHeight = ControllerHelpers.AskNumberInput(min: 5, max: 30);

            Console.Write("Enter grid size (odd number, min 3): ");
            int gridSize;
            while (true)
            {
                gridSize = ControllerHelpers.AskNumberInput(min: 3);
                if (gridSize % 2 == 1 && gridSize <= 25) break;
                Console.WriteLine("Grid size must be an odd number and less than 25. Try again:");
            }

            Console.WriteLine("Choose starting player:");
            Console.WriteLine("1) X starts");
            Console.WriteLine("2) O starts");
            int startingPlayerChoice = ControllerHelpers.AskNumberInput("Choose an option (1 or 2): ", 1, 2);
            EGamePiece startingPlayer = startingPlayerChoice == 1 ? EGamePiece.X : EGamePiece.O;

            var config = _gameService.CreateGameConfiguration(boardWidth, boardHeight, gridSize, startingPlayer);

            Console.WriteLine($"Number of pieces per player will be {config.NumberOfPieces}.");

            return config;
        }
    }
}
