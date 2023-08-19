// BASEDON: https://github.com/dotnet/runtime/blob/bd83e17052d3c09022bad1d91dca860ca6b27ab9/src/libraries/System.Private.CoreLib/src/System/Numerics/Hashing/HashHelpers.cs
// ReSharper disable All
#pragma warning disable IDE0007
#if NETCOREAPP30_OR_GREATER || NETSTANDARD21_OR_GREATER || LESSTHAN_NET45
#else
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Numerics.Hashing
{
	internal static class HashHelpers
	{
		public static int Combine(int h1, int h2)
		{
			// RyuJIT optimizes this to use the ROL instruction
			// Related GitHub pull request: https://github.com/dotnet/coreclr/pull/1830
			uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
			return ((int)rol5 + h1) ^ h2;
		}
	}
}
#endif