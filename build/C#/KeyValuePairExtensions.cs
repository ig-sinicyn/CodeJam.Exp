// BASEDON: https://github.com/dotnet/runtime/blob/bd83e17052d3c09022bad1d91dca860ca6b27ab9/src/libraries/System.Collections.Immutable/src/System/Polyfills.cs#L2
// ReSharper disable All
#if NETCOREAPP20_OR_GREATER || NETSTANDARD21_OR_GREATER || LESSTHAN_NET45
#else
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Collections.Generic
{
	internal static class KeyValuePairExtensions
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> source, out TKey key, out TValue value)
		{
			key = source.Key;
			value = source.Value;
		}
	}
}
#endif