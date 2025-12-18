using api.Models;
using dataccess;
using Dtos;

namespace api.Services;

public interface IBoardService
{
    Task<Guid> CreateBet(CreateBetDto dto, Guid userId, Transaction transaction);

    Task<PagedResult<BoardDto>> GetAllBoards();

    Task<PagedResult<BoardDto>> GetBoardsByUser(Guid userId);
}