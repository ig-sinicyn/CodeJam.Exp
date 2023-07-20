using System.Threading.Tasks;

namespace CodeJam.Tests;

[TestFixture]
public class TargetingTests
{
#if TARGETS_NETCOREAPP || TARGETS_NETSTANDARD || NET40_OR_GREATER
	[Test]
	public void TestSpans()
	{
		System.MemoryExtensions
		var a = "Hello!".AsSpan();

		a[1..^2].Should().BeEquivalentTo("ell");
	}

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
	public async IAsyncEnumerable<string?> EnumerateAsync()
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
}