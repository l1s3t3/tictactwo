﻿using GameBrain;
using DAL;
using DTO;
using ConsoleUI;

namespace ConsoleApp
{
    public static class GameController
    {
        private static IConfigRepository _configRepo = new ConfigRepository();
        private static IGameRepository? _gameRepository;
        private static GameService? _gameService;
        private static GamePhases? _gamePhases;
        private static SaveController? _saveController;
        private static ConfController? _confController;
        private static AiController? _aiController;

        public static void SetGameRepository(IGameRepository repository)
        {
            _gameRepository = repository;
            _gameService = new GameService(_gameRepository, _configRepo);
            _saveController = new SaveController(_gameService);
            _confController = new ConfController(_gameService, _saveController);
            _gamePhases = new GamePhases(_gameService);
            _aiController = new AiController(_gameService);
        }
        
        public static void MainLoop(bool playWithAi, bool aiVsAi, EGamePiece aiPlayerPiece)
        {
            if (_gameRepository == null || _gameService == null || _confController == null || _saveController == null || _gamePhases == null)
            {
                throw new InvalidOperationException("GameController is not fully initialized");
            }
            
            _confController.SelectGameConfiguration();
            
            var gameState = _gameService.GetCurrentGameState();
            gameState.PlayWithAI = playWithAi;
            gameState.AiVsAi = aiVsAi;
            gameState.AIPlayerPiece = aiPlayerPiece;
            _gameService.SetCurrentGameState(gameState);

        
            Console.WriteLine($"Each player has {_gameService.NumberOfPieces} pieces.");

            RunMainLoop();
        }

        public static void RunMainLoop()
        {
            if (_gameRepository == null || _gameService == null || _confController == null || _saveController == null || _gamePhases == null)
            {
                throw new InvalidOperationException("GameController is not fully initialized");
            }

            while (true)
            {
                if (GameSettings.ActivateConsoleClear)
                {
                    Console.Clear();
                }

                Visualizer.DrawBoard(_gameService);

                int remainingPieces = _gameService.NumberOfPieces - (_gameService.CurrentPlayer == EGamePiece.X
                    ? _gameService.TotalPiecesX
                    : _gameService.TotalPiecesO);

                Console.WriteLine($"{_gameService.CurrentPlayer}'s turn. You have {remainingPieces} piece(s) left.");
                
                
                if (_aiController != null && _aiController.IsAiPlayer(_gameService.CurrentPlayer))
                {
                    while (true)
                    {
                        Console.WriteLine("1) Next AI Move");
                        Console.WriteLine("2) Save Game");
                        Console.WriteLine("3) Exit Game");
                        Console.Write("Choose an option: ");
                        var input = Console.ReadLine()?.Trim();
                        if (input == "1")
                        {
                            MoveType moveType;
                            if (_gameService.InitialPlacementPhase)
                            {
                                moveType = MoveType.PlacePiece;
                            }
                            else
                            {
                                moveType = _aiController.GetAiMoveType();
                            }

                            bool moveResult = _aiController.MakeAiMove(moveType);
                            if (!moveResult)
                            {
                                Console.WriteLine("AI made an invalid move (check your AI logic)!");
                                Console.ReadLine();
                                return;
                            }
                            break;
                        }
                        else if (input == "2")
                        {
                            _saveController.SaveGame();
                            Console.WriteLine("Game saved. Press Enter to continue...");
                            Console.ReadLine();
                        }
                        else if (input == "3")
                        {
                            return; 
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice, please try again.");
                        }
                    }
                }
                else
                {
                    MoveType moveType;
                    if (_gameService.InitialPlacementPhase)
                    {

                        moveType = _gamePhases.HandleInitialPlacementPhase(); // paigutusfaas
                        if (moveType == MoveType.ExitGame)
                        {
                            break;
                        }
                        else if (moveType == MoveType.SaveGame)
                        {
                            _saveController.SaveGame();
                            continue;
                        }
                    }
                    else
                    {

                        moveType = _gamePhases.HandleMainGamePhase(); // põhifaas
                        if (moveType == MoveType.ExitGame)
                        {
                            break;
                        }
                        else if (moveType == MoveType.SaveGame)
                        {
                            _saveController.SaveGame();
                            continue;
                        }
                    }

                    bool moveResult = MakeMove(moveType);

                    if (!moveResult)
                    {
                        Console.WriteLine("Invalid move, please try again.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        continue;
                    }
                }

                var winner = _gameService.CheckWinCondition();
                if (winner != null)
                {
                    if (GameSettings.ActivateConsoleClear)
                    {
                        Console.Clear();
                    }
                    Visualizer.DrawBoard(_gameService);

                    if (winner == EGamePiece.Empty)
                    {
                        Console.WriteLine("The game is a tie!");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Player {winner} wins!");
                        Console.WriteLine();
                    }

                    break;
                }
                
                
            }
        }
        
        private static bool MakeMove(MoveType moveType)
        {
            if (_gameService == null)
            {
                throw new InvalidOperationException("GameService is not initialized. Make sure to call SetGameRepository first.");
            }
            
            bool moveResult = false;

            if (moveType == MoveType.PlacePiece)
            {
                if (GameSettings.ActivateConsoleClear)
                {
                    Console.Clear();
                }
                Visualizer.DrawBoard(_gameService);

                int remainingPieces = _gameService.NumberOfPieces - (_gameService.CurrentPlayer == EGamePiece.X
                    ? _gameService.TotalPiecesX
                    : _gameService.TotalPiecesO);

                Console.WriteLine($"{_gameService.CurrentPlayer}'s turn. You have {remainingPieces} piece(s) left.");

                var (x, y) = ControllerHelpers.AskCoordinateInput(
                    "Enter coordinate within the grid (example: A0): ",
                    (x, y) => _gameService.GameBoard[x, y] == EGamePiece.Empty && _gameService.IsWithinGrid(x, y),
                    "Invalid move: The cell is either occupied or outside the active grid.",
                    _gameService.DimX,
                    _gameService.DimY);

                moveResult = _gameService.PlacePiece(x, y);
            }
            else if (moveType == MoveType.MoveGrid)
            {
                List<GridDirection> validDirections = _gameService.GetValidGridMovements();

                if (validDirections.Count == 0)
                {
                    Console.WriteLine("No valid grid movements are available.");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    return false;
                }

                Console.WriteLine("Available grid movement directions:");
                for (int i = 0; i < validDirections.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {validDirections[i]}");
                }

                int directionChoice = ControllerHelpers.AskNumberInput($"Choose a direction (1-{validDirections.Count}): ", 1,
                    validDirections.Count);

                GridDirection selectedDirection = validDirections[directionChoice - 1]; // Valik teisendatakse indeksi alusel suunaks

                moveResult = _gameService.MoveGrid(selectedDirection);
            }
            else if (moveType == MoveType.MovePiece)
            {
                var (fromX, fromY) = ControllerHelpers.AskCoordinateInput(
                    "From (example: A0): ",
                    (x, y) => _gameService.GameBoard[x, y] == _gameService.CurrentPlayer && _gameService.IsWithinBounds(x, y),
                    "Invalid source: The selected piece is not yours or outside the board.",
                    _gameService.DimX,
                    _gameService.DimY);

                var (toX, toY) = ControllerHelpers.AskCoordinateInput(
                    "To (example: A1): ",
                    (x, y) => _gameService.GameBoard[x, y] == EGamePiece.Empty && _gameService.IsWithinGrid(x, y),
                    "Invalid destination: The cell is either wrong, occupied or outside the active grid.",
                    _gameService.DimX,
                    _gameService.DimY);

                moveResult = _gameService.MovePiece(fromX, fromY, toX, toY);
            }

            return moveResult;
        }
        
    }
}