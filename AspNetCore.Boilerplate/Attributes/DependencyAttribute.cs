﻿namespace AspNetCore.Boilerplate;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DependencyAttribute(Lifetime lifetime) : Attribute
{
    public Lifetime Lifetime { get; } = lifetime;

    /// <summary>
    /// Register services as itself
    /// </summary>
    public bool IsSelf { get; init; }
}
