using System.Diagnostics.CodeAnalysis;

namespace CodeJam.Strings;

public static partial class StringExtensions
{
	/// <summary>
	/// Infix form of <see cref="string.IsNullOrEmpty"/>.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns><c>true</c> if <paramref name="str"/> is null or empty; otherwise, <c>false</c>.</returns>
	[Pure, ContractsPure]
	public static bool IsNullOrEmpty([NotNullWhen(false)] this string? str)
	{
		// DONTTOUCH: Do not remove return statements
		// https://github.com/dotnet/coreclr/issues/914

		if (str == null || 0u >= (uint)str.Length)
			return true;

		return false;
	}

	/// <summary>
	/// Returns true if argument is not null nor empty.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns><c>true</c> if <paramref name="str"/> is not null nor empty; otherwise, <c>false</c>.</returns>
	[Pure, ContractsPure]
	public static bool NotNullNorEmpty([NotNullWhen(true)] this string? str) => !str.IsNullOrEmpty();

	/// <summary>
	/// Concatenates all the elements of a string array, using the specified separator between each element.
	/// </summary>
	/// <remarks>
	/// Infix form of <see cref="string.Join(string,string[])"/>.
	/// </remarks>
	/// <param name="values">An array that contains the elements to concatenate.</param>
	/// <param name="separator">
	/// The string to use as a separator. <paramref name="separator"/> is included in the returned string only
	/// if <paramref name="values"/> has more than one element.
	/// </param>
	/// <returns>
	/// A string that consists of the members of <paramref name="values"/> delimited by the <paramref name="separator"/>
	/// string.
	/// If <paramref name="values"/> has no members, the method returns <see cref="string.Empty"/>.
	/// </returns>
	[Pure, ContractsPure]
	public static string Join(this string?[] values, string? separator) =>
		string.Join(separator, values);

#if !(NET20 || NET30 || NET35)

	/// <summary>
	/// Concatenates the members of a constructed <see cref="IEnumerable{T}"/> collection of type <see cref="string"/>,
	/// using the specified separator between each member.
	/// </summary>
	/// <remarks>
	/// Infix form of string.Join(string,IEnumerable{string}).
	/// </remarks>
	/// <param name="values">A collection that contains the strings to concatenate.</param>
	/// <param name="separator">
	/// The string to use as a separator. <paramref name="separator"/> is included in the returned string only
	/// if <paramref name="values"/> has more than one element.
	/// </param>
	/// <returns>
	/// A string that consists of the members of <paramref name="values"/> delimited by the <paramref name="separator"/>
	/// string.
	/// If <paramref name="values"/> has no members, the method returns <see cref="string.Empty"/>.
	/// </returns>
	[Pure, ContractsPure]
	public static string Join([InstantHandle] this IEnumerable<string?> values, string? separator) =>
		// ReSharper disable once BuiltInTypeReferenceStyle
		String.Join(
			separator
#if LESSTHAN_NET47
					!
#endif
			,
			values
#if LESSTHAN_NET47
					!
#endif
			);

#endif

	/// <summary>
	/// Concatenates the members of a collection, using the specified separator between each member.
	/// </summary>
	/// <typeparam name="T">Type of value</typeparam>
	/// <param name="values">A collection that contains the strings to concatenate.</param>
	/// <param name="separator">
	/// The string to use as a separator. <paramref name="separator"/> is included in the returned string only
	/// if <paramref name="values"/> has more than one element.
	/// </param>
	/// <returns>
	/// A string that consists of the members of <paramref name="values"/> delimited by the <paramref name="separator"/>
	/// string.
	/// If <paramref name="values"/> has no members, the method returns <see cref="string.Empty"/>.
	/// </returns>
	[Pure, ContractsPure]
	public static string Join<T>([InstantHandle] this IEnumerable<T> values, string? separator) =>
#if !(NET20 || NET30 || NET35)
		// ReSharper disable once BuiltInTypeReferenceStyle
		String.Join(
			separator
#if LESSTHAN_NET47
					!
#endif
			,
			values);
#else // No covariant IEnumerable
			values is IEnumerable<string> strValues ? StringEx.Join(separator!, strValues) : StringEx.Join(separator!, values);
#endif

	/// <summary>
	/// Concatenates the members of a collection.
	/// </summary>
	/// <typeparam name="T">Type of value</typeparam>
	/// <param name="values">A collection that contains the strings to concatenate.</param>
	/// <returns>
	/// A string that consists of the members of <paramref name="values"/>.
	/// If <paramref name="values"/> has no members, the method returns <see cref="string.Empty"/>.
	/// </returns>
	[Pure, ContractsPure]
	public static string Join<T>([InstantHandle] this IEnumerable<T> values) =>
		// ReSharper disable once BuiltInTypeReferenceStyle
		StringEx.Join("", values);
}