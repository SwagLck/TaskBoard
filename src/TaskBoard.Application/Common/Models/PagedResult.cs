namespace TaskBoard.Application.Common.Models;

public record PagedResult<T>(List<T> items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double) PageSize);
}