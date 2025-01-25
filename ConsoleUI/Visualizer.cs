using DTO;
using GameBrain;

namespace ConsoleUI
{
    public static class Visualizer
    {
        public static void DrawBoard(GameService gameService)
        {
            var board = gameService.GameBoard;
            int width = gameService.DimX;
            int height = gameService.DimY;
            int centerX = gameService.CenterX;
            int centerY = gameService.CenterY;
            int gridSize = gameService.GridSize;
            int halfGrid = gridSize / 2;

            Console.ResetColor();

            // X-numbers
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   ");
            for (int x = 0; x < width; x++)
            {
                Console.Write($"{x,2} ");
            }
            Console.WriteLine();
            Console.ResetColor();

            for (int y = 0; y < height; y++)
            {
                // Y-letters
                Console.ForegroundColor = ConsoleColor.Yellow;
                char yChar = (char)('A' + y); // Convert 0 -> 'A', 1 -> 'B', etc.
                Console.Write($" {yChar} ");
                Console.ResetColor();

                for (int x = 0; x < width; x++)
                {
                    bool isInGrid = Math.Abs(x - centerX) <= halfGrid && Math.Abs(y - centerY) <= halfGrid;

                    var piece = board[x, y];

                    if (isInGrid)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray; // grid cells
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black; // non-grid cells
                    }

                    if (piece == EGamePiece.X)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"[X]");
                    }
                    else if (piece == EGamePiece.O)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"[O]");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White; // Empty cell color
                        Console.Write(isInGrid ? "[ ]" : " . "); // Represent empty cells differently if within the grid
                    }

                    Console.ResetColor();
                }
                Console.WriteLine();
            }

            Console.ResetColor();
        }
    }
}
