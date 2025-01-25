namespace DTO
{
    public record struct GameConfiguration()
    {
        public string Name { get; set; } = default!;
        public int BoardSizeWidth { get; set; } = 5;
        public int BoardSizeHeight { get; set; } = 5;
        public int GridSize { get; set; } = 3;
        public int NumberOfPieces { get; set; } = 4;
        public int WinCondition { get; set; } = 3;

        public override string ToString() =>
            $"Board: {BoardSizeWidth}x{BoardSizeHeight}, Grid: {GridSize}x{GridSize}, Pieces per Player: {NumberOfPieces}, Win Condition: {WinCondition}";
    }
    
}