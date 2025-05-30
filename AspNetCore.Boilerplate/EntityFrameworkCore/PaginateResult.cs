namespace AspNetCore.Boilerplate.EntityFrameworkCore;

public class PaginateResult<TEntity>
{
    public required int Count { get; init; }

    public required IAsyncEnumerable<TEntity> Data { get; init; }
}
