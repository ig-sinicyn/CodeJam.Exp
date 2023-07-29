﻿#if TARGETS_NET || LESSTHAN_NETSTANDARD21 || LESSTHAN_NETCOREAPP30
// ReSharper disable once CheckNamespace
// BASEDON: https://gist.github.com/bgrainger/fb2c18659c2cdfce494c82a8c4803360
// (taken from https://github.com/bgrainger/IndexRange readme)
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.CompilerServices;

internal static class RuntimeHelpers
{
	/// <summary>
	/// Slices the specified array using the specified range.
	/// </summary>
	public static T[] GetSubArray<T>(T[] array, Range range)
	{
		if (array == null)
		{
			throw new ArgumentNullException(nameof(array));
		}

		(var offset, var length) = range.GetOffsetAndLength(array.Length);

		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		if (default(T) != null ||
			typeof(T[]) ==
			array.GetType())
		{
			// We know the type of the array to be exactly T[].
			if (length == 0)
			{
				return ArrayEx.Empty<T>();
			}

			var dest = new T[length];
			Array.Copy(array, offset, dest, 0, length);
			return dest;
		}
		else
		{
			// The array is actually a U[] where U:T.
			var dest = (T[])Array.CreateInstance(array.GetType().GetElementType()!, length);
			Array.Copy(array, offset, dest, 0, length);
			return dest;
		}
	}
}
#endif