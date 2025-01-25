using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Configuration
{
    public int Id { get; set; }

    [MaxLength(128)]
    public string Name { get; set; } = default!;

    public int BoardWidth { get; set; }
    public int BoardHeight { get; set; }

    public ICollection<SaveGame>? SaveGames { get; set; }
    
}