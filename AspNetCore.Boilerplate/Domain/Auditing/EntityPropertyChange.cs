namespace AspNetCore.Boilerplate.Domain.Auditing;

public class EntityPropertyChange : Entity<int>
{
    public required string Entity { get; init; }

    public required string? NewValue { get; init; }

    public required string? OriginalValue { get; init; }

    public required string PropertyName { get; init; }

    public required string PropertyTypeFullName { get; init; }
}
