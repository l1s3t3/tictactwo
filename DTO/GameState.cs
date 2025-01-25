namespace DTO
{
    public class GameState
    {
        public GameConfiguration GameConfiguration { get; set; }
        public EGamePiece[][] GameBoard { get; set; }
        public EGamePiece NextMoveBy { get; set; }
        public int TotalPiecesX { get; set; }
        public int TotalPiecesO { get; set; }
        public int CenterX { get; set; }
        public int CenterY { get; set; }
        public int LastMoveType { get; set; }
        public EGamePiece LastPlayer { get; set; }
        
        public bool InitialPlacementPhase { get; set; }
        
        public bool PlayWithAI { get; set; }
        public bool AiVsAi { get; set; }
        public EGamePiece AIPlayerPiece { get; set; }
    }
}