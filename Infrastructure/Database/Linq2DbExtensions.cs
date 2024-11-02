using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace LinqToDB;

public static class Linq2DbExtensions
{
    public static async Task<ImmutableArray<TSource>> ToImmutableArrayAsync<TSource>(
        this IQueryable<TSource> source, 
        CancellationToken cancellationToken)
    {
        var list = await source.ToListAsync(cancellationToken);
        TSource[] array = new TSource[list.Count];
        CollectionsMarshal.AsSpan(list).CopyTo(array);
        var result = ImmutableCollectionsMarshal.AsImmutableArray(array);

        return result;
    }
}
