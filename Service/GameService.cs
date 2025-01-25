using DAL;
using System;
using System.Collections.Generic;
using GameBrain;

namespace Service
{
    public class GameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IConfigRepository _configRepository;
        private TicTacTwoBrain _gameInstance;
        private GameState _loadedGameState;
        private EGamePiece _startingPlayer = EGamePiece.X;
        public bool InitialPlacementPhase { get; set; } = true;

        public GameService(IGameRepository gameRepository, IConfigRepository configRepository)
        {
            _gameRepository = gameRepository;
            _configRepository = configRepository;
        }

        // Mängu initsialiseerimine
        public void InitializeGame(GameConfiguration config, GameState loadedGameState = null)
        {
            _gameInstance = new TicTacTwoBrain(config);

            if (loadedGameState != null)
            {
                _gameInstance.SetGameState(loadedGameState);
                InitialPlacementPhase = loadedGameState.InitialPlacementPhase;
                _loadedGameState = null; // Reset for next game
            }
            else
            {
                _gameInstance.CurrentPlayer = _startingPlayer;
            }
        }

        // Mängu konfiguratsiooni loomine
        public GameConfiguration CreateGameConfiguration(int boardWidth, int boardHeight, int gridSize, EGamePiece startingPlayer)
        {
            _startingPlayer = startingPlayer;

            return new GameConfiguration
            {
                BoardSizeWidth = boardWidth,
                BoardSizeHeight = boardHeight,
                GridSize = gridSize,
                NumberOfPieces = gridSize + 1,
            };
        }

        // Mängu konfiguratsiooni laadimine nime järgi
        public GameConfiguration GetConfigurationByName(string name)
        {
            return _configRepository.GetConfigurationByName(name);
        }

        // Mängu salvestamine
        public void SaveGame(string saveName)
        {
            var gameState = _gameInstance.GetGameState();
            var jsonStateString = System.Text.Json.JsonSerializer.Serialize(gameState);
            _gameRepository.SaveGame(jsonStateString, saveName);
        }

        // Mängu laadimine
        public GameState LoadGame(string gameName)
        {
            var jsonStateString = _gameRepository.LoadGame(gameName);
            if (jsonStateString == null)
            {
                throw new Exception("Error loading the saved game.");
            }

            var gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(jsonStateString);
            return gameState;
        }

        // Kontrollib, kas mäng eksisteerib
        public bool GameExists(string gameName)
        {
            return _gameRepository.GameExists(gameName);
        }

        // Saab salvestatud mängude nimed
        public List<string?> GetSavedGameNames()
        {
            return _gameRepository.GetSavedGameNames();
        }

        // Kustutab mängu
        public void DeleteGame(string gameName)
        {
            _gameRepository.DeleteGame(gameName);
        }

        // Mängu käigu tegemine
        public bool MakeMove(int moveType, params object[] parameters)
        {
            return _gameInstance.MakeAMove(moveType, parameters);
        }

        // Mängu oleku saamine
        public GameState GetGameState()
        {
            return _gameInstance.GetGameState();
        }

        // Võidutingimuse kontrollimine
        public EGamePiece? CheckWinCondition()
        {
            return _gameInstance.CheckWinCondition();
        }

        // Saadaval olevad grid-liikumise suunad
        public List<GridDirection> GetValidGridMovements()
        {
            return _gameInstance.GetValidGridMovements();
        }

        // Mängija käigu tegemine
        public bool PlacePiece(int x, int y)
        {
            return _gameInstance.MakeAMove(1, x, y);
        }

        // Grid'i liigutamine
        public bool MoveGrid(GridDirection direction)
        {
            return _gameInstance.MakeAMove(2, direction);
        }

        // Nupu liigutamine
        public bool MovePiece(int fromX, int fromY, int toX, int toY)
        {
            return _gameInstance.MakeAMove(3, fromX, fromY, toX, toY);
        }

        // Getterid mängu oleku kohta
        public EGamePiece CurrentPlayer => _gameInstance.CurrentPlayer;
        public int TotalPiecesX => _gameInstance.TotalPiecesX;
        public int TotalPiecesO => _gameInstance.TotalPiecesO;
        public int NumberOfPieces => _gameInstance.NumberOfPieces;
        public EGamePiece[,] GameBoard => _gameInstance.GameBoard;
        public int DimX => _gameInstance.DimX;
        public int DimY => _gameInstance.DimY;
        public int CenterX => _gameInstance.CenterX;
        public int CenterY => _gameInstance.CenterY;
        public int GridSize => _gameInstance.GridSize;
        public GameConfiguration GameConfiguration => _gameInstance.GameConfiguration;

        // Lisame meetodid IsWithinGrid ja IsWithinBounds, kui neid vajatakse
        public bool IsWithinGrid(int x, int y)
        {
            return _gameInstance.IsWithinGrid(x, y);
        }

        public bool IsWithinBounds(int x, int y)
        {
            return _gameInstance.IsWithinBounds(x, y);
        }
    }
}
