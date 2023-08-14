// BASEDON: https://gist.github.com/bgrainger/fb2c18659c2cdfce494c82a8c4803360
// (taken from https://github.com/bgrainger/IndexRange readme)
// ReSharper disable All
#if NETCOREAPP30_OR_GREATER || NETSTANDARD21_OR_GREATER
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Runtime.CompilerServices.RuntimeHelpers))]
#else
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable once CheckNamespace
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
#if TARGETS_NETCOREAPP || NETSTANDARD13_OR_GREATER || NET46_OR_GREATER
				return Array.Empty<T>();
#else
				return new T[0];
#endif
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

	// Used for pinning strings (e.g. `fixed (char* res = result) {}`)
	[Obsolete("OffsetToStringData has been deprecated. Use string.GetPinnableReference() instead.")]
	public static int OffsetToStringData
	{
		// This offset is baked in by string indexer intrinsic, so there is no harm
		// in getting it baked in here as well.
		get =>
			// Number of bytes from the address pointed to by a reference to
			// a String to the first 16-bit character in the String.  Skip
			// over the MethodTable pointer, & String
			// length.  Of course, the String reference points to the memory
			// after the sync block, so don't count that.
			// This property allows C#'s fixed statement to work on Strings.
			// On 64 bit platforms, this should be 12 (8+4) and on 32 bit 8 (4+4).
			IntPtr.Size == 8 ? 12 : 8;
	}
}
#endif