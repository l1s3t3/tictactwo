using MenuSystem;
using DTO;

namespace ConsoleApp
{
    public static class Menus
    {
        public static readonly Menu MainMenu =
            new Menu(
                EMenuLevel.Main,
                "TIC-TAC-TWO",
                new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "R",
                        Title = "Rules",
                        MenuItemAction = () =>
                        {
                            
                            Console.Clear();
                            Console.WriteLine();
                            Console.WriteLine("Tic-Tac-Two Rules:");
                            Console.WriteLine();
                            Console.WriteLine("Tic-Tac-Two is played on a board with a movable tic-tac-toe grid, which is at first centered at the center of the board.");
                            Console.WriteLine("The players \"X\" and \"O\" have both gridSize + 1 pieces to place.");
                            Console.WriteLine();
                            Console.WriteLine("At the start, each player takes turns placing one of their pieces on any empty cell within the tic-tac-toe grid.");
                            Console.WriteLine("Once each player has placed at least two of their pieces, they may do one of three things on their turn:");
                            Console.WriteLine("(1) place one of their remaining pieces on an empty cell within the tic-tac-toe grid,");
                            Console.WriteLine("(2) move the tic-tac-toe grid any available ways away from the cell it was originally centered at, or");
                            Console.WriteLine("(3) move one of their pieces that is on the board or grid to any empty cell within the grid.");
                            Console.WriteLine("The first player to create a horizontal, vertical, or diagonal line of their pieces in the tic-tac-toe grid wins.");
                            Console.WriteLine();
                            Console.WriteLine("Press 'R' to return to the main menu.");
                            string? input;
                            do
                            {
                                input = Console.ReadLine();
                            } while (input?.ToUpper() != "R");
                            return string.Empty;
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "N",
                        Title = "New game",
                        MenuItemAction = () =>
                        {
                            GameController.MainLoop(false, false, EGamePiece.O);
                            return string.Empty; // Return to the Main Menu
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "P",
                        Title = "Play with AI",
                        MenuItemAction = () =>
                        {
                            Console.WriteLine("Do you want the AI to play as X or O?");
                            Console.WriteLine("1) X");
                            Console.WriteLine("2) O");

                            int choice = ControllerHelpers.AskNumberInput("Choose AI's piece (1 or 2): ", 1, 2);
                            var aiPiece = choice == 1 ? EGamePiece.X : EGamePiece.O;

                            // Käivitame MainLoop koos parameetritega, mis ütlevad, et on PvAI
                            GameController.MainLoop(true, false, aiPiece);
                            return string.Empty;
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "I",
                        Title = "AI vs AI",
                        MenuItemAction = () =>
                        {
                            GameController.MainLoop(true, true, EGamePiece.X);
                            return string.Empty;
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "A",
                        Title = "Activate Console Clear",
                        MenuItemAction = () =>
                        {
                            GameSettings.ActivateConsoleClear = !GameSettings.ActivateConsoleClear;
                            Console.WriteLine($"Console Clear in game is now {(GameSettings.ActivateConsoleClear ? "activated" : "deactivated")}.");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            return string.Empty;
                        }
                    },

                });
    }
}
