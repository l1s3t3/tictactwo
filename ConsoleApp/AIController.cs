using GameBrain;
using DTO;

namespace ConsoleApp
{
    public class AiController
    {
        private readonly GameService _gameService;
        
        public AiController(GameService gameService)
        {
            _gameService = gameService;
        }

        public bool IsAiPlayer(EGamePiece player)
        {
            var gameState = _gameService.GetCurrentGameState();
            if (gameState.AiVsAi) return true;
            return gameState.PlayWithAI && player == gameState.AIPlayerPiece;
        }


        public MoveType GetAiMoveType()
        {
            return AIPlayer.ChooseMoveType(_gameService);
        }

        public bool MakeAiMove(MoveType moveType)
        {
            bool moveResult = false;

            if (moveType == MoveType.PlacePiece)
            {
                var placement = AIPlayer.ChoosePlacement(_gameService);
                if (placement != null && _gameService.IsWithinGrid(placement.Value.x, placement.Value.y))
                {
                    moveResult = _gameService.PlacePiece(placement.Value.x, placement.Value.y);
                }
            }
            else if (moveType == MoveType.MoveGrid)
            {
                var direction = AIPlayer.ChooseGridMovement(_gameService);
                if (direction != null)
                {
                    moveResult = _gameService.MoveGrid(direction.Value);
                }
            }
            else if (moveType == MoveType.MovePiece)
            {
                var pieceMove = AIPlayer.ChoosePieceMove(_gameService);
                if (pieceMove != null && _gameService.GameBoard[pieceMove.Value.fromX, pieceMove.Value.fromY] == _gameService.CurrentPlayer)
                {
                    moveResult = _gameService.MovePiece(pieceMove.Value.fromX, pieceMove.Value.fromY, pieceMove.Value.toX, pieceMove.Value.toY);
                }
                else
                {
                    Console.WriteLine("AI cannot find a valid piece to move.");
                    return false;
                }
            }

            return moveResult;
        }
    }
}
