using System.Reflection;
using System.Runtime.Versioning;

namespace CodeJam.Targeting.Tests;

[TestFixture]
public class EnvironmentTests
{
	private const string _expectedCiAssemblyVersionRegex = @"^1\.0\.\d+\.0$";

	private const string _expectedLocalAssemblyVersionRegex = @"^1\.0\.0\.0$";

	private const string _expectedTargetFramework =
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

	private const string _expectedRuntimeRegex =
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

	private static readonly bool _runsInPipeline = Environment.GetEnvironmentVariable("CI") != null;

	[Test]
	public void TargetAssemblyVersion_MatchesExpected()
	{
		var targetVersion = typeof(TargetingFeatures)
			.Assembly
			.GetName().Version?.ToString();

		var versionRegex = _runsInPipeline
			? _expectedCiAssemblyVersionRegex
			: _expectedLocalAssemblyVersionRegex;

		targetVersion.Should().MatchRegex(versionRegex);

		Console.WriteLine(targetVersion);
	}

	[Test]
	public void TargetAssemblyFramework_MatchesExpected()
	{
		var targetFramework = typeof(TargetingFeatures)
			.Assembly
			.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
			?? "N/A";

		targetFramework.Should().Be(_expectedTargetFramework);
	}

	[Test]
	public void TestAssemblyVersion_MatchesExpected()
	{
		var targetVersion = GetType()
			.Assembly
			.GetName().Version?.ToString();

		var versionRegex = _runsInPipeline
			? _expectedCiAssemblyVersionRegex
			: _expectedLocalAssemblyVersionRegex;

		targetVersion.Should().MatchRegex(versionRegex);
	}

	[Test]
	public void TestAssemblyFramework_MatchesExpected()
	{
		var targetFramework = GetType()
			.Assembly
			.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
			?? "N/A";

		targetFramework.Should().Be(_expectedTargetFramework);
	}

	[Test]
	public void RuntimeVersion_MatchesExpected()
	{
		var targetRuntime = typeof(int)
			.Assembly
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
			?? "N/A";

		targetRuntime.Should().MatchRegex(_expectedRuntimeRegex);
	}
}