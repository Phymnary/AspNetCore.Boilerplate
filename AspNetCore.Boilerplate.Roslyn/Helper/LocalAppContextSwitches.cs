// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

// ReSharper disable All

namespace AspNetCore.Boilerplate.Roslyn.Helper;

internal static partial class LocalAppContextSwitches
{
    private static int s_useNonRandomizedHashSeed;

    public static bool UseNonRandomizedHashSeed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get =>
            GetCachedSwitchValue(
                "Switch.System.Data.UseNonRandomizedHashSeed",
                ref s_useNonRandomizedHashSeed
            );
    }

    // Returns value of given switch using provided cache.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool GetCachedSwitchValue(string switchName, ref int cachedSwitchValue)
    {
        // The cached switch value has 3 states: 0 - unknown, 1 - true, -1 - false
        if (cachedSwitchValue < 0)
            return false;
        if (cachedSwitchValue > 0)
            return true;

        return GetCachedSwitchValueInternal(switchName, ref cachedSwitchValue);
    }

    private static bool GetCachedSwitchValueInternal(string switchName, ref int cachedSwitchValue)
    {
        bool isSwitchEnabled;

        var hasSwitch = AppContext.TryGetSwitch(switchName, out isSwitchEnabled);
        if (!hasSwitch)
            isSwitchEnabled = GetSwitchDefaultValue(switchName);

        AppContext.TryGetSwitch(
            @"TestSwitch.LocalAppContext.DisableCaching",
            out var disableCaching
        );
        if (!disableCaching)
            cachedSwitchValue = isSwitchEnabled
                ? 1 /*true*/
                : -1 /*false*/
            ;

        return isSwitchEnabled;
    }

    // Provides default values for switches if they're not always false by default
    private static bool GetSwitchDefaultValue(string switchName)
    {
        if (switchName == "Switch.System.Runtime.Serialization.SerializationGuard")
            return true;

        return false;
    }
}
