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
			".NETFramework,Version=v4.7.2";
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

	[Test]
	public void TargetAttributeTest()
	{
		var targetFramework = typeof(TargetingFeatures)
			.Assembly
			.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
			?? "N/A";

		targetFramework.Should().Be(ExpectedTargetFramework);
	}

	/*
#if NET50_OR_GREATER
	[Test]
	public void TestSpans()
	{
		var a = "Hello!".AsSpan();

		a[1..^2].ToString().Should().BeEquivalentTo("ell");
	}
#endif

#if TARGETS_NETCOREAPP || TARGETS_NETSTANDARD || NET40_OR_GREATER
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public record Record(string Value)
	{
		public string? InitValue { get; init; }
	}

	[Test]
	public void TestRecord()
	{
		var record = new Record("sample") { InitValue = "sample2" };

		record.Value.Should().Be("sample");
		record.InitValue.Should().Be("sample2");
	}
#endif

	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public readonly record struct RecordStruct(string Value)
	{
		public string? InitValue { get; init; } = null;
	}

	[Test]
	public void TestRecordStruct()
	{
		var record = new RecordStruct("sample") { InitValue = "sample2" };

		record.Value.Should().Be("sample");
		record.InitValue.Should().Be("sample2");
	}

#if NET45_OR_GREATER
	private async IAsyncEnumerable<string?> EnumerateAsync()
	{
		await TaskEx.Delay(1);
		yield return "Hello!";
	}

	[Test]
	public async Task TestAsyncEnumerable()
	{
		await foreach (var s in EnumerateAsync())
		{
			s.Should().Be("Hello!");
		}
	}
#endif

	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	internal class CSharp10Features
	{
		[Pure, ContractsPure]
		public async Task<string?> TaskAsync()
		{
			await TaskEx.Yield();
			return "Hello!";
		}

		public async ValueTask<string?> ValueTaskAsync()
		{
			await TaskEx.Yield();
			return "Hello!";
		}
	}

	[Test]
	public void TestTasks()
	{
		var t = TaskEx.Run(() => 42);
		var r = t.Result;

		r.Should().Be(42);
	}

	[Test]
	public void TestTuple()
	{
		var a = (a: 1, b: 2, c: 3);

		a.b.Should().Be(2);
	}

#if LESSTHAN_NET45
	[Test]
	public void TestAsyncShim() => TestAsync().Wait();

#else
	[Test]
#endif
	public async Task TestAsync()
	{
		var sample = new CSharp10Features();
		(await sample.TaskAsync()).Should().Be("Hello!");
		(await sample.ValueTaskAsync()).Should().Be("Hello!");
	}

	[Test]
	public void TestIndex()
	{
		var bytes = new byte[] { 1, 2, 3 };

		bytes[1..].Should().BeEquivalentTo(new byte[] { 2, 3 });

		var text = "Hello!";

		text[1..^1].Should().Be("ello");
	}

	[Test]
	public void TestIndexTypes()
	{

		var bytes = new byte[] { 1, 2, 3 };

		var index = new System.Range(1, Index.End);
		bytes[index].Should().BeEquivalentTo(new byte[] { 2, 3 });

		var text = "Hello!";

		var textIndex = new System.Range(1, ^1);
		text[textIndex].Should().Be("ello");
	}

	[Test]
	public void TestKeyValuePairDeconstruct()
	{
		var pair = new KeyValuePair<int, string>(1, "2");

		var (a, b) = pair;

		a.Should().Be(1);
		b.Should().Be("2");
	}

	[Test]
	public static void PrintQuirks()
	{
		var assembly = typeof(int).Assembly;

		var desc =
#if TARGETS_NETCOREAPP || TARGETS_NETSTANDARD || NET46_OR_GREATER
			System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
#else
			"no description";
#endif
		Console.WriteLine($"Runtime v{Environment.Version} ({desc})");
		Console.WriteLine($"{PlatformHelper.TargetPlatform}. Running on {assembly}");
		Console.WriteLine();
		PrintProps("System.Runtime.Versioning.BinaryCompatibility");
		Console.WriteLine();
		PrintProps("System.CompatibilitySwitches");
		Console.WriteLine();
		PrintProps("System.AppContextSwitches");
	}

	private static void PrintProps(string typeName)
	{
		var type = typeof(int).Assembly.GetType(typeName);
		if (type == null)
		{
			Console.WriteLine($"No type {typeName} found.");
			return;
		}

		Console.WriteLine(type.Name);
		var bf = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		foreach (var prop in type.GetProperties(bf))
		{
			Console.WriteLine($"\t * {prop.Name}: {prop.GetValue(null, null)}");
		}
	}*/
}