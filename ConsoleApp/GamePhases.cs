using DTO;
using GameBrain;

namespace ConsoleApp;

public class GamePhases
{
    private readonly GameService _gameService;

    public GamePhases(GameService gameService)
    {
        _gameService = gameService;
    }

    public MoveType HandleInitialPlacementPhase()
        {
            Console.WriteLine("Options: ");

            Dictionary<int, MoveType> optionToMoveType = new Dictionary<int, MoveType>();
            int optionNumber = 1;

            optionToMoveType[optionNumber] = MoveType.PlacePiece;
            Console.WriteLine($"{optionNumber}) Place piece");
            optionNumber++;

            optionToMoveType[optionNumber] = MoveType.SaveGame;
            Console.WriteLine($"{optionNumber}) Save game");
            optionNumber++;

            optionToMoveType[optionNumber] = MoveType.ExitGame;
            Console.WriteLine($"{optionNumber}) Exit game");

            var availableOptions = new List<int>(optionToMoveType.Keys);

            int choice = ControllerHelpers.AskOptionInput($"Choose your action ({string.Join("/", availableOptions)}): ",
                availableOptions);

            MoveType moveType = optionToMoveType[choice];

            if (moveType == MoveType.ExitGame)
            {
                Console.WriteLine("Exiting the game...");
            }
            else if (moveType == MoveType.SaveGame)
            {
                return MoveType.SaveGame;
            }

            return moveType;
        }


    public MoveType HandleMainGamePhase()
        {
            Console.WriteLine("Options: ");

            Dictionary<int, MoveType> optionToMoveType = new Dictionary<int, MoveType>();
            int optionNumber = 1;

            optionToMoveType[optionNumber] = MoveType.SaveGame;
            Console.WriteLine($"{optionNumber}) Save game");
            optionNumber++;

            if ((_gameService.CurrentPlayer == EGamePiece.X && _gameService.TotalPiecesX < _gameService.NumberOfPieces) ||
                (_gameService.CurrentPlayer == EGamePiece.O && _gameService.TotalPiecesO < _gameService.NumberOfPieces))
            {
                optionToMoveType[optionNumber] = MoveType.PlacePiece;
                Console.WriteLine($"{optionNumber}) Place piece");
                optionNumber++;
            }

            optionToMoveType[optionNumber] = MoveType.MoveGrid;
            Console.WriteLine($"{optionNumber}) Move grid");
            optionNumber++;

            optionToMoveType[optionNumber] = MoveType.MovePiece;
            Console.WriteLine($"{optionNumber}) Move piece");
            optionNumber++;

            optionToMoveType[optionNumber] = MoveType.ExitGame;
            Console.WriteLine($"{optionNumber}) Exit game");

            var availableOptions = new List<int>(optionToMoveType.Keys);

            int choice = ControllerHelpers.AskOptionInput($"Choose your action ({string.Join("/", availableOptions)}): ",
                availableOptions);

            MoveType moveType = optionToMoveType[choice];

            if (moveType == MoveType.ExitGame)
            {
                Console.WriteLine("Exiting the game...");
            }
            else if (moveType == MoveType.SaveGame)
            {
                return MoveType.SaveGame;
            }

            return moveType;
        }
    
}