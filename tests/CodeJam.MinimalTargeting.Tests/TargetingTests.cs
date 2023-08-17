using NUnit.Framework.Internal;

using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace CodeJam.Targeting.Tests;

[TestFixture]
public class TargetingTests
{
	/// <summary>
	/// The expected is the same or lower than the current target depending on our build.
	/// </summary>
	public const string ExpectedTargetFramework =
#if NET20
		".NETFramework,Version=v2.0";
#elif NET30
		".NETFramework,Version=v3.0";
#elif NET35
		".NETFramework,Version=v3.5";
#elif NET40
		".NETFramework,Version=v4.0";
#elif NET45
		".NETFramework,Version=v4.5";
#elif NET451
		".NETFramework,Version=v4.5.1";
#elif NET452
		".NETFramework,Version=v4.5.2";
#elif NET46
		".NETFramework,Version=v4.6";
#elif NET461
		".NETFramework,Version=v4.6.1";
#elif NET462
		".NETFramework,Version=v4.6.1";
#elif NET47
		".NETFramework,Version=v4.6.1";
#elif NET471
		".NETFramework,Version=v4.6.1";
#elif NET472
		".NETFramework,Version=v4.7.2";
#elif NET48
		".NETFramework,Version=v4.8";
#elif NET5_0 || NETCOREAPP5_0
		".NETCoreApp,Version=v5.0";
#elif NET6_0
		".NETCoreApp,Version=v6.0";
#elif NET7_0
		".NETCoreApp,Version=v7.0";
#elif NETCOREAPP1_0
		".NETCoreApp,Version=v1.0";
#elif NETCOREAPP1_1
		".NETCoreApp,Version=v1.0";
#elif NETCOREAPP2_1
		".NETCoreApp,Version=v2.0";
#elif NETCOREAPP3_0
		".NETCoreApp,Version=v3.0";
#elif NETCOREAPP3_1
		".NETCoreApp,Version=v3.1";
#else
		"UNKNOWN";
#endif

	public const string ExpectedRuntimeRegex =
#if NET20
		"(^2\\.0)|(^4\\.8)";
#elif NET30
		"(^2\\.0)|(^4\\.8)";
#elif NET35
		"(^2\\.0)|(^4\\.8)";
#elif NET40
		"^4\\.8";
#elif NET45
		"^4\\.8";
#elif NET451
		"^4\\.8";
#elif NET452
		"^4\\.8";
#elif NET46
		"^4\\.8";
#elif NET461
		"^4\\.8";
#elif NET462
		"^4\\.8";
#elif NET47
		"^4\\.8";
#elif NET471
		"^4\\.8";
#elif NET472
		"^4\\.8";
#elif NET48
		"^4\\.8";
#elif NET5_0 || NETCOREAPP5_0
		"^5\\.0";
#elif NET6_0
		"^6\\.0";
#elif NET7_0
		"^7\\.0";
#elif NETCOREAPP1_0
		"^1\\.1";
#elif NETCOREAPP1_1
		"^1\\.2";
#elif NETCOREAPP2_1
		"^2\\.1";
#elif NETCOREAPP3_0
		"^3\\.0";
#elif NETCOREAPP3_1
		"^3\\.1";
#else
		"UNKNOWN";
#endif

	[Test]
	public void TargetAssemblyFramework_Matches()
	{
		var targetFramework = typeof(TargetingFeatures)
			.Assembly
			.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
			?? "N/A";

		targetFramework.Should().Be(ExpectedTargetFramework);
	}

	[Test]
	public void TestAssemblyFramework_Matches()
	{
		var targetFramework = GetType()
			.Assembly
			.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
			?? "N/A";

		targetFramework.Should().Be(ExpectedTargetFramework);
	}

	[Test]
	public void RuntimeVersion_Matches()
	{
		var targetRuntime = typeof(int)
			.Assembly
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
			?? "N/A";

		targetRuntime.Should().MatchRegex(ExpectedRuntimeRegex);
	}
}