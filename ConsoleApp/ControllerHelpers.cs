
namespace ConsoleApp
{
    public static class ControllerHelpers
    {
        public static int AskNumberInput(string prompt = "", int min = int.MinValue, int max = int.MaxValue)
        {
            if (!string.IsNullOrEmpty(prompt)) Console.WriteLine(prompt);

            int number;
            while (!int.TryParse(Console.ReadLine(), out number) || number < min || number > max)
            {
                Console.WriteLine($"Invalid input. Please enter a number between {min} and {max}:");
            }

            return number;
        }

        public static int AskOptionInput(string prompt, List<int> validOptions)
        {
            if (!string.IsNullOrEmpty(prompt)) Console.WriteLine(prompt);

            int number;
            while (true)
            {
                string? input = Console.ReadLine();
                if (!int.TryParse(input, out number))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }
                if (!validOptions.Contains(number))
                {
                    Console.WriteLine($"Invalid choice. Please choose from the following options: {string.Join(", ", validOptions)}");
                    continue;
                }
                break;
            }
            return number;
        }

        public static (int x, int y) AskCoordinateInput(
            string prompt,
            Func<int, int, bool> isValidCoordinate,
            string errorMessage,
            int boardWidth,
            int boardHeight)
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(prompt)) Console.WriteLine(prompt);
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
                {
                    Console.WriteLine("Invalid input. Please enter a coordinate like A0, B3, etc.");
                    continue;
                }

                char yChar = char.ToUpper(input[0]);
                string xStr = input.Substring(1);

                if (!char.IsLetter(yChar))
                {
                    Console.WriteLine("Invalid input. The first character should be a letter (A, B, C, ...).");
                    continue;
                }

                if (!int.TryParse(xStr, out int x))
                {
                    Console.WriteLine("Invalid input. The numeric part is invalid.");
                    continue;
                }

                int y = yChar - 'A';

                if (x < 0 || x >= boardWidth)
                {
                    Console.WriteLine($"Invalid input. X coordinate should be between 0 and {boardWidth - 1}.");
                    continue;
                }

                if (y < 0 || y >= boardHeight)
                {
                    Console.WriteLine($"Invalid input. Y coordinate should be between A and {(char)('A' + boardHeight - 1)}.");
                    continue;
                }

                if (!isValidCoordinate(x, y))
                {
                    Console.WriteLine(errorMessage);
                    continue;
                }

                return (x, y);
            }
        }
    }
}
