
using DTO;

namespace DAL
{
    public class ConfigRepository : IConfigRepository
    {
        private List<GameConfiguration> _gameConfigurations = new List<GameConfiguration>()
        {
            new GameConfiguration()
            {
                Name = "Classical",
                BoardSizeWidth = 5,
                BoardSizeHeight = 5,
                GridSize = 3,
                NumberOfPieces = 4,
            },
            // Other configurations can be added here
        };

        public List<string> GetConfigurationNames()
        {
            return _gameConfigurations
                .OrderBy(x => x.Name)
                .Select(config => config.Name)
                .ToList();
        }

        public GameConfiguration GetConfigurationByName(string name)
        {
            return _gameConfigurations.Single(c => c.Name == name);
        }
    }
}