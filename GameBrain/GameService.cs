using DAL;
using DTO;
using System.Text.Json;

namespace GameBrain
{
    public class GameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IConfigRepository _configRepository;
        private TicTacTwoBrain? _gameInstance;
        private EGamePiece _startingPlayer = EGamePiece.X;
        public bool InitialPlacementPhase => _gameInstance?.InitialPlacementPhase ?? false;
        
        public GameService(IGameRepository gameRepository, IConfigRepository configRepository)
        {
            _gameRepository = gameRepository;
            _configRepository = configRepository;
        }
        
        public void InitializeGame(GameConfiguration config, GameState? loadedGameState = null)
        {
           
            _gameInstance = new TicTacTwoBrain(config);

            if (loadedGameState != null)
            {
                _gameInstance.SetGameState(loadedGameState);
            }
            else
            {
                _gameInstance.CurrentPlayer = _startingPlayer;
            }
        }
        
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
        
        public GameConfiguration GetConfigurationByName(string name)
        {
            return _configRepository.GetConfigurationByName(name);
        }
        
        public void SaveGame(string saveName, string? playerX = null, string? playerO = null)
        {
            var gameState = GameInstance.GetGameState();
            var jsonStateString = JsonSerializer.Serialize(gameState);
            _gameRepository.SaveGame(jsonStateString, saveName, playerX, playerO);
        }

        
        public (GameState? gameState, string? playerX, string? playerO) LoadGame(string gameName)
        {
            var (jsonStateString, playerX, playerO) = _gameRepository.LoadGame(gameName);
            if (jsonStateString == null)
            {
                return (null, null, null);
            }

            var gameState = JsonSerializer.Deserialize<GameState>(jsonStateString);
            if (gameState == null)
            {
                return (null, null, null);
            }

            return (gameState, playerX, playerO);
        }
        
        public bool GameExists(string gameName)
        {
            return _gameRepository.GameExists(gameName);
        }
        
        public GameState GetCurrentGameState()
        {
            return GameInstance.GetGameState();
        }
        
        public void SetCurrentGameState(GameState gameState)
        {
            GameInstance.SetGameState(gameState);
        }
        
        public List<string> GetSavedGameNames()
        {
            return _gameRepository.GetSavedGameNames();
        }
        
        public void DeleteGame(string gameName)
        {
            _gameRepository.DeleteGame(gameName);
        }
        
        public bool MakeMove(int moveType, params object[] parameters)
        {
            return GameInstance.MakeAMove(moveType, parameters);
        }
        
        public EGamePiece? CheckWinCondition()
        {
            return GameInstance.CheckWinCondition();
        }
        
        public List<GridDirection> GetValidGridMovements()
        {
            return GameInstance.GetValidGridMovements();
        }
        
        public bool PlacePiece(int x, int y)
        {
            return GameInstance.MakeAMove(1, x, y);
        }
        
        public void UndoLastMove()
        {
            GameInstance.UndoLastMove();
        }
        
        public bool MoveGrid(GridDirection direction)
        {
            return GameInstance.MakeAMove(2, direction);
        }
        
        public bool MovePiece(int fromX, int fromY, int toX, int toY)
        {
            return GameInstance.MakeAMove(3, fromX, fromY, toX, toY);
        }
        
        public EGamePiece[,] GetGridSection()
        {
            return GameInstance.GetGridSection();
        }
        public EGamePiece CurrentPlayer => GameInstance.CurrentPlayer;
        public int TotalPiecesX => GameInstance.TotalPiecesX;
        public int TotalPiecesO => GameInstance.TotalPiecesO;
        public int NumberOfPieces => GameInstance.NumberOfPieces;
        public EGamePiece[,] GameBoard => GameInstance.GameBoard;
        public int DimX => GameInstance.DimX;
        public int DimY => GameInstance.DimY;
        public int CenterX => GameInstance.CenterX;
        public int CenterY => GameInstance.CenterY;
        public int GridSize => GameInstance.GridSize;

        
        public bool IsWithinGrid(int x, int y)
        {
            return GameInstance.IsWithinGrid(x, y);
        }

        public bool IsWithinBounds(int x, int y)
        {
            return GameInstance.IsWithinBounds(x, y);
        }
        
        private TicTacTwoBrain GameInstance => _gameInstance 
                                               ?? throw new InvalidOperationException("GameInstance is not initialized.");


    }
}
