using DTO;

namespace GameBrain

{
    public class TicTacTwoBrain
    {
        private EGamePiece[,] _gameBoard;
        private int _gridSize;
        private GameConfiguration _gameConfiguration;
        private int _lastMoveType;
        private EGamePiece _lastPlayer;
        
        public TicTacTwoBrain(GameConfiguration gameConfiguration)
        {
            _gameConfiguration = gameConfiguration;
            _gridSize = _gameConfiguration.GridSize;
            _gameBoard = new EGamePiece[gameConfiguration.BoardSizeWidth, gameConfiguration.BoardSizeHeight];
            CenterX = gameConfiguration.BoardSizeWidth / 2;
            CenterY = gameConfiguration.BoardSizeHeight / 2;
            CurrentPlayer = EGamePiece.X;
        }
        
        public bool PlayWithAI { get; private set; }
        public bool AiVsAi { get; private set; }
        public EGamePiece AIPlayerPiece { get; private set; }
        
        public int TotalPiecesX { get; private set; }

        public int TotalPiecesO { get; private set; }

        public bool InitialPlacementPhase { get; private set; } = true;

        public int NumberOfPieces => _gameConfiguration.NumberOfPieces;

        public int CenterX { get; private set; }

        public int CenterY { get; private set; }

        public int GridSize => _gridSize;

        public EGamePiece CurrentPlayer { get; set; }
        public EGamePiece[,] GameBoard => _gameBoard;
        public int DimX => _gameBoard.GetLength(0);
        public int DimY => _gameBoard.GetLength(1);
        
        // for AI!
        private int _lastFromX, _lastFromY;
        private int _lastToX, _lastToY;
        private int _lastCenterX, _lastCenterY;


        public bool MakeAMove(int moveType, params object[] parameters)
        {
            _lastMoveType = moveType;
            _lastPlayer = CurrentPlayer;

            bool result = false;

            if (moveType == 1 && parameters.Length == 2)
            {
                int x = (int)parameters[0];
                int y = (int)parameters[1];
                
                result = PlacePiece(x, y);
            }
            else if (moveType == 2 && parameters.Length == 1)
            {
                GridDirection direction = (GridDirection)parameters[0];
                
                _lastCenterX = CenterX;
                _lastCenterY = CenterY;

                result = MoveGrid(direction);
            }
            else if (moveType == 3 && parameters.Length == 4)
            {
                int fromX = (int)parameters[0];
                int fromY = (int)parameters[1];
                int toX = (int)parameters[2];
                int toY = (int)parameters[3];
                
                _lastFromX = fromX;
                _lastFromY = fromY;
                _lastToX = toX;
                _lastToY = toY;

                result = MovePiece(fromX, fromY, toX, toY);
            }

            if (!result)
            {
                _lastMoveType = 0;
            }

            return result;
        }

        private bool PlacePiece(int x, int y)
        {
            if (CurrentPlayer == EGamePiece.X && TotalPiecesX >= _gameConfiguration.NumberOfPieces)
            {
                Console.WriteLine("You have no remaining pieces to place.");
                return false;
            }

            if (CurrentPlayer == EGamePiece.O && TotalPiecesO >= _gameConfiguration.NumberOfPieces)
            {
                Console.WriteLine("You have no remaining pieces to place.");
                return false;
            }

            if (_gameBoard[x, y] != EGamePiece.Empty || !IsWithinGrid(x, y))
            {
                Console.WriteLine("Invalid move: Either the cell is occupied or outside the active grid.");
                return false;
            }

            _gameBoard[x, y] = CurrentPlayer;
            if (CurrentPlayer == EGamePiece.X)
            {
                TotalPiecesX++;
            }
            else
            {
                TotalPiecesO++;
            }
            if (TotalPiecesX >= 2 && TotalPiecesO >= 2)
            {
                InitialPlacementPhase = false;
            }
            
            _lastToX = x;
            _lastToY = y;
            
            CurrentPlayer = CurrentPlayer == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
            return true;
        }

        public List<GridDirection> GetValidGridMovements()
        {
            List<GridDirection> validDirections = new List<GridDirection>();

            if (CanMoveGrid(-1, 0)) validDirections.Add(GridDirection.Left);
            if (CanMoveGrid(1, 0)) validDirections.Add(GridDirection.Right);
            if (CanMoveGrid(0, -1)) validDirections.Add(GridDirection.Up);
            if (CanMoveGrid(0, 1)) validDirections.Add(GridDirection.Down);
            if (CanMoveGrid(-1, -1)) validDirections.Add(GridDirection.UpLeft);
            if (CanMoveGrid(1, -1)) validDirections.Add(GridDirection.UpRight);
            if (CanMoveGrid(-1, 1)) validDirections.Add(GridDirection.DownLeft);
            if (CanMoveGrid(1, 1)) validDirections.Add(GridDirection.DownRight);

            return validDirections;
        }

        private bool CanMoveGrid(int dx, int dy)
        {
            int newCenterX = CenterX + dx; // gridi uus keskpunkt pärast liigutamist
            int newCenterY = CenterY + dy;
            int halfGrid = _gridSize / 2; // et arvutada gridi servade asukoht
            
            return newCenterX - halfGrid >= 0 && // Vasakserv on korras?
                   newCenterX + halfGrid < DimX && // Paremserv on korras?
                   newCenterY - halfGrid >= 0 && // Ülemine serv on korras?
                   newCenterY + halfGrid < DimY;
        }

        private bool MoveGrid(GridDirection direction)
        {
            int dx = 0;
            int dy = 0;

            switch (direction) // olenevalt, mille mängija valis, anname dx ja dy väärtused
            {
                case GridDirection.Up:
                    dy = -1;
                    break;
                case GridDirection.Down:
                    dy = 1;
                    break;
                case GridDirection.Left:
                    dx = -1;
                    break;
                case GridDirection.Right:
                    dx = 1;
                    break;
                case GridDirection.UpLeft:
                    dx = -1;
                    dy = -1;
                    break;
                case GridDirection.UpRight:
                    dx = 1;
                    dy = -1;
                    break;
                case GridDirection.DownLeft:
                    dx = -1;
                    dy = 1;
                    break;
                case GridDirection.DownRight:
                    dx = 1;
                    dy = 1;
                    break;
            }

            if (CanMoveGrid(dx, dy))
            {
                CenterX += dx;
                CenterY += dy;
                
                CurrentPlayer = CurrentPlayer == EGamePiece.X ? EGamePiece.O : EGamePiece.X;

                return true;
            }
            else
            {
                Console.WriteLine("The grid cannot move outside the bounds.");
                return false;
            }
        }

        private bool MovePiece(int fromX, int fromY, int toX, int toY)
        {
            if (!IsWithinBounds(fromX, fromY))
            {
                Console.WriteLine("Choose again! This is outside the board.");
                return false;
            }

            if (_gameBoard[fromX, fromY] != CurrentPlayer)
            {
                Console.WriteLine("The selected piece is not yours.");
                return false;
            }

            if (!IsWithinGrid(toX, toY))
            {
                Console.WriteLine("Invalid: The cell is outside the active grid.");
                return false;
            }

            if (_gameBoard[toX, toY] != EGamePiece.Empty)
            {
                Console.WriteLine("Invalid: The destination cell is occupied.");
                return false;
            }

            _gameBoard[fromX, fromY] = EGamePiece.Empty;
            _gameBoard[toX, toY] = CurrentPlayer;
            
            CurrentPlayer = CurrentPlayer == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
            return true;
        }

        public EGamePiece? CheckWinCondition()
        {
            var grid = GetGridSection();
            bool xWins = CheckWin(grid, EGamePiece.X);
            bool oWins = CheckWin(grid, EGamePiece.O);

            if (xWins && oWins)
            {
                if (_lastMoveType == 2) //Mõlemal on võiduvõimalus
                {
                    return _lastPlayer; // Kui viimane mängija liigutas gridi, siis tema võidab
                }
                else
                {
                    return EGamePiece.Empty; //Viik
                }
            }
            else if (xWins)
            {
                return EGamePiece.X;
            }
            else if (oWins)
            {
                return EGamePiece.O;
            }

            return null;
        }


        public EGamePiece[,] GetGridSection()
        {
            int halfGrid = _gridSize / 2;
            var grid = new EGamePiece[_gridSize, _gridSize]; // salvestatakse grid
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    int boardX = CenterX - halfGrid + i; // mängulaua absoluutne koordinaat
                    int boardY = CenterY - halfGrid + j;

                    if (IsWithinBounds(boardX, boardY))
                    {
                        grid[i, j] = _gameBoard[boardX, boardY];
                    }
                    else
                    {
                        grid[i, j] = EGamePiece.Empty;
                    }
                }
            }
            return grid;
        }

        private bool CheckWin(EGamePiece[,] grid, EGamePiece piece)
        {
            for (int i = 0; i < _gridSize; i++)
            {
                bool rowWin = true;
                bool colWin = true;
                for (int j = 0; j < _gridSize; j++)
                {
                    if (grid[i, j] != piece) rowWin = false; // kas on õige nupp
                    if (grid[j, i] != piece) colWin = false;
                }

                if (rowWin || colWin) return true;
            }

            bool diagWin1 = true;
            bool diagWin2 = true;
            for (int i = 0; i < _gridSize; i++)
            {
                if (grid[i, i] != piece) diagWin1 = false;
                if (grid[i, _gridSize - i - 1] != piece) diagWin2 = false;
            }

            return diagWin1 || diagWin2;
        }

        public bool IsWithinGrid(int x, int y)
        {
            int halfGrid = _gridSize / 2;
            return x >= CenterX - halfGrid && x <= CenterX + halfGrid &&
                   y >= CenterY - halfGrid && y <= CenterY + halfGrid;
        }

        public bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < DimX && y >= 0 && y < DimY;
        }
        
        public void UndoLastMove()
        {
            switch (_lastMoveType)
            {
                case 1:
                    _gameBoard[_lastToX, _lastToY] = EGamePiece.Empty;
                    if (_lastPlayer == EGamePiece.X) TotalPiecesX--;
                    else TotalPiecesO--;
                    CurrentPlayer = _lastPlayer;
                    break;

                case 2:
                    CenterX = _lastCenterX;
                    CenterY = _lastCenterY;
                    CurrentPlayer = _lastPlayer;
                    break;

                case 3:
                    _gameBoard[_lastFromX, _lastFromY] = _lastPlayer;
                    _gameBoard[_lastToX, _lastToY] = EGamePiece.Empty;
                    CurrentPlayer = _lastPlayer;
                    break;

                default:
                    Console.WriteLine("No move to undo.");
                    break;
            }
        }
        
        
        private EGamePiece[][] ConvertToJaggedArray(EGamePiece[,] multiArray)
        {
            int rows = multiArray.GetLength(0);
            int cols = multiArray.GetLength(1);
            EGamePiece[][] jaggedArray = new EGamePiece[rows][];
            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new EGamePiece[cols];
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = multiArray[i, j];
                }
            }

            return jaggedArray;
        }
        
        private EGamePiece[,] ConvertToMultiArray(EGamePiece[][] jaggedArray) // tagasi mängulauaks
        {
            int rows = jaggedArray.Length;
            int cols = jaggedArray[0].Length;
            EGamePiece[,] multiArray = new EGamePiece[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    multiArray[i, j] = jaggedArray[i][j];
                }
            }

            return multiArray;
        }

        public GameState GetGameState() // sisaldab kõike vajalikku, et mäng salvestada
        {
            return new GameState
            {
                GameConfiguration = _gameConfiguration,
                GameBoard = ConvertToJaggedArray(_gameBoard),
                NextMoveBy = CurrentPlayer,
                TotalPiecesX = TotalPiecesX,
                TotalPiecesO = TotalPiecesO,
                CenterX = CenterX,
                CenterY = CenterY,
                LastMoveType = _lastMoveType,
                LastPlayer = _lastPlayer,
                InitialPlacementPhase = InitialPlacementPhase,
                PlayWithAI = PlayWithAI,
                AiVsAi = AiVsAi,
                AIPlayerPiece = AIPlayerPiece
            };
        }


        public void SetGameState(GameState state)  // kui mängu laadida, on vaja määrata väljad salvestatud andmete põhjal
        {
            _gameConfiguration = state.GameConfiguration;
            _gridSize = _gameConfiguration.GridSize;
            _gameBoard = ConvertToMultiArray(state.GameBoard);
            CurrentPlayer = state.NextMoveBy;
            TotalPiecesX = state.TotalPiecesX;
            TotalPiecesO = state.TotalPiecesO;
            CenterX = state.CenterX;
            CenterY = state.CenterY;
            _lastMoveType = state.LastMoveType;
            _lastPlayer = state.LastPlayer;
            InitialPlacementPhase = state.InitialPlacementPhase;
            PlayWithAI = state.PlayWithAI;
            AiVsAi = state.AiVsAi;
            AIPlayerPiece = state.AIPlayerPiece;
        }

    }
}