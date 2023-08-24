using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Versioning;

namespace CodeJam
{
	internal static class PlatformHelper
	{
		/// <summary>Target platform the assembly was built for.</summary>
		// ReSharper disable once ConstantConditionalAccessQualifier
		public static readonly string? TargetPlatform =
			typeof(PlatformHelper).Assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
	}
}
