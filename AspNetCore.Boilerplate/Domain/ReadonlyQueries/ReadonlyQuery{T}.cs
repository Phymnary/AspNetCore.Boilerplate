using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Boilerplate.Domain.ReadonlyQueries;

public class ReadonlyQuery<T>
{
    private readonly IQueryable<T> _queryable;

    internal ReadonlyQuery(IQueryable<T> queryable)
    {
        _queryable = queryable;
    }

    public ReadonlyQuery<TTarget> Select<TTarget>(Expression<Func<T, TTarget>> selector)
    {
        return new ReadonlyQuery<TTarget>(_queryable.Select(selector));
    }

    public IAsyncEnumerable<T> AsAsyncEnumerable()
    {
        return _queryable.AsAsyncEnumerable();
    }

    public Task<List<T>> ToListAsync()
    {
        return _queryable.ToListAsync();
    }
    
    public Task<T[]> ToArrayAsync()
    {
        return _queryable.ToArrayAsync();
    }
}