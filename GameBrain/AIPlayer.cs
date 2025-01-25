using DTO;

namespace GameBrain
{
    public static class AIPlayer
    {
        private static Random _random = new Random();

        public static MoveType ChooseMoveType(GameService gameService)
        {
            if (gameService.InitialPlacementPhase)
            {
                return MoveType.PlacePiece;
            }

            bool canPlace = CanPlaceMorePieces(gameService);
            if (canPlace)
            {
                return MoveType.PlacePiece;
            }

            int gridScore = EvaluateGrid(gameService);
            if (gameService.CurrentPlayer == EGamePiece.X && gridScore < 0)
            {
                var gridMove = ChooseGridMovement(gameService);
                if (gridMove != null) return MoveType.MoveGrid;
            }
            
            if (gameService.CurrentPlayer == EGamePiece.O && gridScore > 0)
            {
                var gridMove = ChooseGridMovement(gameService);
                if (gridMove != null) return MoveType.MoveGrid;
            }


            var pieceMove = ChoosePieceMove(gameService);
            if (pieceMove != null) return MoveType.MovePiece;

            var fallbackGridMove = ChooseGridMovement(gameService);
            if (fallbackGridMove != null) return MoveType.MoveGrid;

            return MoveType.MovePiece;
        }


        private static bool CanPlaceMorePieces(GameService gameService)
        {
            return (gameService.CurrentPlayer == EGamePiece.X &&
                    gameService.TotalPiecesX < gameService.NumberOfPieces) ||
                   (gameService.CurrentPlayer == EGamePiece.O && gameService.TotalPiecesO < gameService.NumberOfPieces);
        }

        public static (int fromX, int fromY, int toX, int toY)? ChoosePieceMove(GameService gameService)
        {
            var pieceLocations = new List<(int x, int y)>(); // juba paigutatud tükkide asukoha määramine
            for (int x = 0; x < gameService.DimX; x++)
            {
                for (int y = 0; y < gameService.DimY; y++)
                {
                    if (gameService.GameBoard[x, y] == gameService.CurrentPlayer)
                    {
                        pieceLocations.Add((x, y));
                    }
                }
            }

            var emptyGridCells = GetEmptyGridCells(gameService); 
            if (pieceLocations.Count == 0 || emptyGridCells.Count == 0) return null;

            foreach (var from in pieceLocations) // võidukäigu otsimine
            {
                foreach (var to in emptyGridCells)
                {
                    if (gameService.MakeMove(3, from.x, from.y, to.x, to.y))

                    {
                        // Käik õnnestus, kontrollime võitu
                        if (gameService.CheckWinCondition() == gameService.CurrentPlayer)
                        {
                            gameService.UndoLastMove();
                            return (from.x, from.y, to.x, to.y);
                        }
                        
                        gameService.UndoLastMove();
                    }
                }
            }
            
            foreach (var from in pieceLocations) // tee suvaline kehtiv käik
            {
                foreach (var to in emptyGridCells)
                {
                    if (gameService.MakeMove(3, from.x, from.y, to.x, to.y))
                    {
                        gameService.UndoLastMove();
                        return (from.x, from.y, to.x, to.y);
                    }
                }
            }

            return null;
        }

        public static (int x, int y)? ChoosePlacement(GameService gameService)
        {
            var emptyGridCells = GetEmptyGridCells(gameService);
            if (emptyGridCells.Count == 0) return null;
            
            foreach (var cell in emptyGridCells)
            {
                if (TestPlacePiece(gameService, cell, out bool win) && win)
                {
                    return cell;
                }
            }

            // blokeeriv käik
            foreach (var cell in emptyGridCells)
            {
                if (TestPlacePiece(gameService, cell, out bool winForOpponent, checkForOpponent: true) && winForOpponent)
                {
                    return cell;
                }
            }
            
            return emptyGridCells[_random.Next(emptyGridCells.Count)];
        }



        public static GridDirection? ChooseGridMovement(GameService gameService)
        {
            var validMoves = gameService.GetValidGridMovements();
            if (validMoves.Count == 0) return null;

            int bestScore = int.MinValue;
            GridDirection? bestMove = null;

            foreach (var move in validMoves)
            {
                if (gameService.MakeMove((int)MoveType.MoveGrid, move))
                {
                    int currentScore = EvaluateGrid(gameService);
                    gameService.UndoLastMove();

                    if (currentScore > bestScore)
                    {
                        bestScore = currentScore;
                        bestMove = move;
                    }
                }
            }

            if (bestMove != null) return bestMove;

            return validMoves[_random.Next(validMoves.Count)];
        }

        public static int EvaluateGrid(GameService gameService)
        {
            int score = 0;
            var grid = gameService.GetGridSection();

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                score += EvaluateLine(grid, i, 0, 0, 1); // rida
                score += EvaluateLine(grid, 0, i, 1, 0); // veerg
            }

            // diagonaalid
            score += EvaluateLine(grid, 0, 0, 1, 1);
            score += EvaluateLine(grid, 0, grid.GetLength(0) - 1, 1, -1);

            return score;
        }

        private static int EvaluateLine(EGamePiece[,] grid, int startX, int startY, int stepX, int stepY)
        {
            int countX = 0;
            int countO = 0;
            int length = grid.GetLength(0);
            int x = startX;
            int y = startY;

            for (int i = 0; i < length; i++)
            {
                if (grid[x, y] == EGamePiece.X) countX++;
                else if (grid[x, y] == EGamePiece.O) countO++;

                x += stepX;
                y += stepY;
            }
            
            // ainult X-id: positiivne
            // ainult O-id: negatiivne
            // Segamini või tühjad: 0
            if (countX > 0 && countO == 0)
                return countX * 10;
            if (countO > 0 && countX == 0)
                return -countO * 10;

            return 0;
        }

        private static List<(int x, int y)> GetEmptyGridCells(GameService gs)
        {
            var list = new List<(int x, int y)>();
            for (int x = 0; x < gs.DimX; x++)
            {
                for (int y = 0; y < gs.DimY; y++)
                {
                    if (gs.GameBoard[x, y] == EGamePiece.Empty && gs.IsWithinGrid(x, y))
                    {
                        list.Add((x, y));
                    }
                }
            }

            return list;
        }


        private static bool TestPlacePiece(GameService gs, (int x, int y) cell, out bool win, bool checkForOpponent = false)
        {
            win = false;
            var currentPlayer = gs.CurrentPlayer;

            if (!gs.MakeMove((int)MoveType.PlacePiece, cell.x, cell.y))
            {
                return false;
            }
            
            var opponent = currentPlayer == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
            win = checkForOpponent ? gs.CheckWinCondition() == opponent : gs.CheckWinCondition() == currentPlayer;

            gs.UndoLastMove();
            return true;
        }
        
    }
}