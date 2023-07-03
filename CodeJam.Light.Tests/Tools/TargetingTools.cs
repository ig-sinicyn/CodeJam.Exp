#if DEFAULT_PLATFORM
#endif

using CodeJam.Strings;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CodeJam.Tests.Tools;

[TestFixture]
[Explicit("Manual run only")]
public class TargetingTools
{
	/// <summary>This test generates content of /build/Props/CodeJam.Targeting.props.</summary>
	[Test]
	public void GenerateTargetingProps()
	{
		var config = GetConfig();
		var emitter = new TargetingPropsEmitter(config, Console.Out);
		emitter.Emit();
	}

	[Flags, UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public enum Shims
	{
		// Flags
		None = 0x0,
		TheraotCore = 0x1,
		ReferenceAssemblies = 0x2,
		SystemDiagnosticsContractsPackage = 0x4,
		SystemDiagnosticsContractsReference = 0x8,
		InitAndNullable = 0x10,
		Package = 0x20,
		Test = 0x40,
		Minimal = 0x80,

		// Presets
		NetCore = None,
		LegacyNetCore = SystemDiagnosticsContractsPackage | TheraotCore,
		NetStandard = InitAndNullable,
		LegacyNetStandard = SystemDiagnosticsContractsPackage | TheraotCore,
		NetFramework = SystemDiagnosticsContractsReference | TheraotCore,
		LegacyNetFramework = SystemDiagnosticsContractsReference | ReferenceAssemblies | TheraotCore,
		PackageAndTest = Package | Test,

		CompatibilityShims = TheraotCore
							 | ReferenceAssemblies
							 | SystemDiagnosticsContractsPackage
							 | SystemDiagnosticsContractsReference
							 | InitAndNullable
	}

	public class TargetingConfig
	{
		public string DefaultFrameworkMoniker { get; init; } = default!;

		public Dictionary<string, Shims> NetCoreShims { get; init; } = new(16);

		public Dictionary<string, Shims> NetStandardShims { get; init; } = new(16);

		public Dictionary<string, Shims> NetFrameworkShims { get; init; } = new(16);
	}

	private static TargetingConfig GetConfig()
	{
		var options = new JsonSerializerOptions
		{
			ReadCommentHandling = JsonCommentHandling.Skip,
			PropertyNameCaseInsensitive = true,
			Converters =
			{
				new JsonStringEnumConverter()
			}
		};

		var stream = typeof(TargetingTools).Assembly
			.GetManifestResourceStream(typeof(TargetingTools), "targeting.json")!;

		return JsonSerializer.Deserialize<TargetingConfig>(stream, options)!;
	}

	private class TargetingPropsEmitter
	{
		private readonly TargetingConfig _config;
		private readonly TextWriter _writer;

		public TargetingPropsEmitter(TargetingConfig config, TextWriter writer)
		{
			_config = config;
			_writer = writer;
		}

		#region Helpers

		private void WriteLine(string message) => _writer.WriteLine(message);

		private void WriteLine(string messageFormat, object value) => _writer.WriteLine(messageFormat, value);

		private static string GetMonikerVersion(string moniker) =>
			moniker switch
			{
				_ when moniker.StartsWith("netstandard", StringComparison.Ordinal) =>
					moniker["netstandard".Length..],
				_ when moniker.StartsWith("netcoreapp", StringComparison.Ordinal) =>
					moniker["netcoreapp".Length..],
				_ when moniker.StartsWith("net", StringComparison.Ordinal) =>
					moniker["net".Length..],
				_ => moniker
			};

		private IEnumerable<KeyValuePair<string, Shims>> OrderByMoniker(Dictionary<string, Shims> monikerShims) =>
			// In most cases dictionary preserves insert order until first reallocation
			// As a caveat we do sort dictionary by keys,
			// but current logic is pretty naive may not work for future moniker formats.
			// If any issue please replace with a proper moniker comparer.
			monikerShims.OrderBy(x => GetMonikerVersion(x.Key));

		private IEnumerable<KeyValuePair<string, Shims>> GetAllMonikerShims() =>
			// In most cases dictionary preserves insert order until first reallocation
			// As a caveat we do sort dictionary by keys,
			// but current logic is pretty naive may not work for future moniker formats.
			// If any issue please replace with a proper moniker comparer.
			_config.NetCoreShims.OrderByDescending(x => GetMonikerVersion(x.Key))
				.Concat(_config.NetStandardShims.OrderByDescending(x => GetMonikerVersion(x.Key)))
				.Concat(_config.NetFrameworkShims.OrderByDescending(x => GetMonikerVersion(x.Key)));

		private string GetTargetFrameworkMonikers(Shims filter) =>
			GetAllMonikerShims()
				.Where(x => x.Value.HasFlag(filter))
				.OrderBy(x => x.Key == _config.DefaultFrameworkMoniker ? 0 : 1)
				.Select(x => x.Key)
				.Join(";");

		private static string FormatFlag(Shims shims, Shims filter) =>
			shims.HasFlag(filter).ToString().ToLowerInvariant();

		#endregion

		public void Emit()
		{
			WriteLine($@"<!-- Generated by {nameof(TargetingTools)}.{nameof(GenerateTargetingProps)}() -->
<!-- Check /CodeJam.Light.Tests/Tools/targeting.json for generation options -->
<!-- BASEDON https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/cross-platform-targeting -->

<Project>");

			EmitDefaultPlatform();
			EmitTargetFrameworks();
			EmitCompatibilityConstants();
			EmitTargetingConstants();

			WriteLine("</Project>");
		}

		private void EmitDefaultPlatform() =>
			WriteLine($@"
	<!-- We DO most development and testing on {_config.DefaultFrameworkMoniker} target -->
	<PropertyGroup Condition="" '$(TargetFramework)' == '{_config.DefaultFrameworkMoniker}' "">
		<DefineConstants>$(DefineConstants);DEFAULT_PLATFORM</DefineConstants>
	</PropertyGroup>");

		private void EmitTargetFrameworks() =>
			WriteLine($@"
	<!-- Templates for <TargetFrameworks/> project property -->
	<!-- Visual Studio and Rider do select the first moniker in list as default target
		therefore we place our default platform ({_config.DefaultFrameworkMoniker}) at start of the list -->
	<PropertyGroup>
		<CopyMeTargetFrameworks>{GetTargetFrameworkMonikers(Shims.Package)}</CopyMeTargetFrameworks>
		<CopyMeMinimalTargetFrameworks>{GetTargetFrameworkMonikers(Shims.Package | Shims.Minimal)}</CopyMeMinimalTargetFrameworks>
		<CopyMeTestTargetFrameworks>{GetTargetFrameworkMonikers(Shims.Test)}</CopyMeTestTargetFrameworks>
		<CopyMeMinimalTestTargetFrameworks>{GetTargetFrameworkMonikers(Shims.Minimal)}</CopyMeMinimalTestTargetFrameworks>
	</PropertyGroup>");

		private void EmitCompatibilityConstants()
		{
			WriteLine(@"
	<!-- Compat shim monikers -->
	<PropertyGroup>
		<UseTheraot>false</UseTheraot>
		<UsePackageReferences>false</UsePackageReferences>
		<UseSystemDiagnosticsPackage>false</UseSystemDiagnosticsPackage>
		<UseSystemDiagnosticsReference>false</UseSystemDiagnosticsReference>
		<UseInitAndNullable>false</UseInitAndNullable>
	</PropertyGroup>");

			var compatShims = GetAllMonikerShims()
				.Where(x => (x.Value & Shims.CompatibilityShims) != 0);

			foreach (var (moniker, shim) in compatShims)
			{
				WriteLine($@"
	<PropertyGroup Condition="" '$(TargetFramework)' == '{moniker}' "">
		<UseTheraot>{FormatFlag(shim, Shims.TheraotCore)}</UseTheraot>
		<UsePackageReferences>{FormatFlag(shim, Shims.ReferenceAssemblies)}</UsePackageReferences>
		<UseSystemDiagnosticsPackage>{FormatFlag(shim, Shims.SystemDiagnosticsContractsPackage)}</UseSystemDiagnosticsPackage>
		<UseSystemDiagnosticsReference>{FormatFlag(shim, Shims.SystemDiagnosticsContractsReference)}</UseSystemDiagnosticsReference>
		<UseInitAndNullable>{FormatFlag(shim, Shims.InitAndNullable)}</UseInitAndNullable>
	</PropertyGroup>");
			}
		}

		private void EmitTargetingConstants()
		{
			EmitTargetingConstants(".Net Core", "TARGETS_NETCOREAPP", _config.NetCoreShims);
			EmitTargetingConstants(".Net Standard", "TARGETS_NETSTANDARD", _config.NetStandardShims);
			EmitTargetingConstants(".Net Framework", "TARGETS_NET", _config.NetFrameworkShims);
		}

		private void EmitTargetingConstants(string description, string platform, Dictionary<string, Shims> monikerShims)
		{
			var monikers = OrderByMoniker(monikerShims).Select(x => x.Key).ToArray();

			WriteLine($@"
	<!-- Monikers for {description} -->");

			var templateBegin = @"	<PropertyGroup Condition="" '$(TargetFramework)' == '{0}' "">";
			var template = @"		<DefineConstants>$(DefineConstants){0}</DefineConstants>";
			var templateEnd = @"	</PropertyGroup>";
			for (var monikerIndex = 0; monikerIndex < monikers.Length; monikerIndex++)
			{
				var target = monikers[monikerIndex];
				var lessThanConstants = monikers
					.Skip(1 + monikerIndex)
					.Select(m => ";LESSTHAN_" + m.ReplaceOrdinal(".", "").ToUpperInvariant())
					.Join();

				var notLessThanConstants = monikers
					.Take(monikerIndex + 1)
					.Select(m => ";" + m.ReplaceOrdinal(".", "").ToUpperInvariant() + "_OR_GREATER")
					.Join();

				WriteLine(templateBegin, target);
				WriteLine(template, ";" + platform);
				if (lessThanConstants.NotNullNorEmpty())
					WriteLine(template, lessThanConstants);
				WriteLine(template, notLessThanConstants);
				WriteLine(templateEnd, target);
			}
		}
	}
}