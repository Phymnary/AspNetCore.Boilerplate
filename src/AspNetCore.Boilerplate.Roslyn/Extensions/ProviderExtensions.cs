using System.Collections.Immutable;
using AspNetCore.Boilerplate.Roslyn.Helper;
using Microsoft.CodeAnalysis;

namespace AspNetCore.Boilerplate.Roslyn.Extensions;

internal static class ProviderExtensions
{
    public static IncrementalValuesProvider<(TKey Key, EquatableArray<TElement> Right)> GroupBy<
        TLeft,
        TRight,
        TKey,
        TElement
    >(
        this IncrementalValuesProvider<(TLeft Left, TRight Right)> source,
        Func<(TLeft Left, TRight Right), TKey> keySelector,
        Func<(TLeft Left, TRight Right), TElement?> elementSelector
    )
        where TLeft : IEquatable<TLeft>
        where TRight : IEquatable<TRight>
        where TKey : IEquatable<TKey>
        where TElement : IEquatable<TElement>
    {
        return source
            .Collect()
            .SelectMany(
                (item, token) =>
                {
                    Dictionary<TKey, ImmutableArray<TElement>.Builder> map = new();

                    foreach ((TLeft, TRight) pair in item)
                    {
                        var key = keySelector(pair);
                        var element = elementSelector(pair);

                        if (!map.TryGetValue(key, out var builder))
                        {
                            builder = ImmutableArray.CreateBuilder<TElement>();

                            map.Add(key, builder);
                        }

                        if (element != null)
                            builder.Add(element);
                    }

                    token.ThrowIfCancellationRequested();

                    ImmutableArray<(TKey Key, EquatableArray<TElement> Elements)>.Builder result =
                        ImmutableArray.CreateBuilder<(TKey, EquatableArray<TElement>)>();

                    foreach (var entry in map)
                        result.Add((entry.Key, entry.Value.ToImmutable()));

                    return result;
                }
            );
    }
}