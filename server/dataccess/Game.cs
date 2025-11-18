namespace dataccess;

public class Game
{
    public string Id { get; set; }

    public DateTime? StartTime { get; set; }

    public bool IsActive { get; set; }

    public string WinningNumbers { get; set; }

    public int Revenue { get; set; }
}