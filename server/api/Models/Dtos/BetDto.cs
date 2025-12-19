namespace Dtos;

public record BoardDto(List<int> Numbers);

public record PlaceBetResponse(
    bool Success,
    string Message,
    Guid BetId,
    string SortedNumbers,
    int Count,
    int Price,
    DateTime CreatedAt
);

public record BetHistoryDto(
    Guid Id,
    string Numbers,
    int Count,
    int Price,
    DateTime Date,
    bool IsWinning,
    Guid? BetSeriesId,
    int? SeriesTotal,
    int? SeriesIndex,
    int? GameWeek,
    int? GameYear,
    DateTime? GameStartTime
);

public record BetHistoryResponse(
    IReadOnlyList<BetHistoryDto> Bets,
    int TotalCount,
    int Page,
    int PageSize
);

public record CreateBetDto(
    List<int> Numbers,
    int Count,
    int Price,
    int RepeatWeeks = 1
)
{

}

public record UpdateBetDto(
    List<int> Numbers
);