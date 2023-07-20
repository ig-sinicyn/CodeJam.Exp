namespace CodeJam.Strings;

/// <summary>
/// <see cref="string"/> class extensions.
/// </summary>
[PublicAPI]
public partial class StringExtensions
{
	public static void TestSpans()
	{
		System.MemoryExtensions
		var a = "Hello!".AsSpan();

		a[1..^2].Should().BeEquivalentTo("ell");
	}


	#region Internal API

	/// <summary>
	/// Replace value using ordinal comparison if target framework supports it.
	/// </summary>
	[Pure, ContractsPure]
	internal static string ReplaceOrdinal(this string str, string oldValue, string newValue) =>
#if NETSTANDARD21_OR_GREATER || NETCOREAPP30_OR_GREATER
		str.Replace(oldValue, newValue, StringComparison.Ordinal)
#else
		str.Replace(oldValue, newValue)
#endif
	;

	/// <summary>
	/// Contains value in struct.
	/// </summary>
	[Pure, ContractsPure]
	internal static bool ContainsOrdinal(this string str, char value) =>
#if NETSTANDARD21_OR_GREATER || NETCOREAPP30_OR_GREATER
		str.Contains(value, StringComparison.Ordinal)
#else
		str.IndexOf(value) >= 0
#endif
	;

	#endregion
}