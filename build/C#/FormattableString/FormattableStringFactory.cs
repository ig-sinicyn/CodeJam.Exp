// BASEDON: https://github.com/dotnet/runtime/blob/bd83e17052d3c09022bad1d91dca860ca6b27ab9/src/libraries/System.Private.CoreLib/src/System/Runtime/CompilerServices/FormattableStringFactory.cs
// ReSharper disable All
#if NETCOREAPP10_OR_GREATER || NETSTANDARD13_OR_GREATER || NET46_OR_GREATER
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Runtime.CompilerServices.FormattableStringFactory))]
#elif LESSTHAN_NET45
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Runtime.CompilerServices.FormattableStringFactory))]
#else
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	/// <summary>
	/// Provides a static method to create a <see cref="FormattableString" /> object from a composite format string and its arguments.
	/// </summary>
	public static class FormattableStringFactory
	{
		/// <summary>
		/// Create a <see cref="FormattableString"/> from a composite format string and object
		/// array containing zero or more objects to format.
		/// </summary>
		public static FormattableString Create(string format, params object?[] arguments)
		{
			if (format == null)
				throw new ArgumentNullException(nameof(format));
			if (arguments == null)
				throw new ArgumentNullException(nameof(arguments));

			return new ConcreteFormattableString(format, arguments);
		}

		private sealed class ConcreteFormattableString : FormattableString
		{
			private readonly string _format;
			private readonly object?[] _arguments;

			internal ConcreteFormattableString(string format, object?[] arguments)
			{
				_format = format;
				_arguments = arguments;
			}

			public override string Format => _format;
			public override object?[] GetArguments() { return _arguments; }
			public override int ArgumentCount => _arguments.Length;
			public override object? GetArgument(int index) { return _arguments[index]; }
			public override string ToString(IFormatProvider? formatProvider) { return string.Format(formatProvider, _format, _arguments); }
		}
	}
}
#endif