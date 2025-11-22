using dataccess;

namespace Dtos;

public class GameDto 
{
    public GameDto(Game entity)
    {
        Id = entity.Id;
        StartTime = entity.StartTime;
        IsActive = entity.IsActive;
        WinningNumbers = entity.WinningNumbers;
        Revenue = entity.Revenue;
    }
    
    public string Id { get; set; }
    public DateTime? StartTime { get; set; }
    public bool IsActive { get; set; }
    public string WinningNumbers { get; set; }
    public int Revenue { get; set; }

}