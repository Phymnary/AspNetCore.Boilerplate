// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Security.Cryptography;

// ReSharper disable All

namespace AspNetCore.Boilerplate.Roslyn.Helper;

internal class Interop
{
    internal static unsafe void GetRandomBytes(byte* buffer, int length)
    {
        if (!LocalAppContextSwitches.UseNonRandomizedHashSeed)
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] tmp = new byte[length];
                rng.GetBytes(tmp);
                Marshal.Copy(tmp, 0, (IntPtr)buffer, length);
            }
        }
    }
}
