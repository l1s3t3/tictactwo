namespace DAL
{
    public interface IGameRepository
    {
        void SaveGame(string jsonStateString, string gameName, string? playerX = null, string? playerO = null);
        bool GameExists(string gameName);
        List<string> GetSavedGameNames();
        List<string> GetUserGames(string userName);
        (string? jsonStateString, string? playerX, string? playerO) LoadGame(string gameName);
        void DeleteGame(string gameName);
    }
}