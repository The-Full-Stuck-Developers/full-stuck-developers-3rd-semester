namespace Dtos;

public record PlaceBetRequest(List<int> Numbers);


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
    DateTime Date
);

// for admin
public record BetHistoryResponse(
    IReadOnlyList<BetHistoryDto> Bets,
    int TotalCount,
    int Page,
    int PageSize
);

public record CurrentGameDto(
    Guid Id,
    bool IsActive,
    DateTime? StartTime,
    int Revenue,
    string? WinningNumbers = null
);