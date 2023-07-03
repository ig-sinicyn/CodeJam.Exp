using System.Threading.Tasks;

namespace CodeJam.Tests;

[TestFixture]
public class TargetingTests
{
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	internal class CSharp10Features
	{
		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		public record Record(string Value)
		{
			public string? InitValue { get; init; }
		}

		[Pure, ContractsPure]
		public async Task<Record?> TaskAsync()
		{
			await Task.Yield();
			return new("Hello!");
		}

		public async ValueTask<string?> ValueTaskAsync()
		{
			await Task.Yield();
			return "Hello!";
		}

		public async IAsyncEnumerable<string?> EnumerateAsync()
		{
			await Task.Yield();
			yield return "Hello!";
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

	[Test]
	public void TestRecord()
	{
		var record = new CSharp10Features.Record("sample") { InitValue = "sample2" };

		record.Value.Should().Be("sample");
		record.InitValue.Should().Be("sample2");
	}

	[Test]
	public async Task TestAsync()
	{
		var sample = new CSharp10Features();
		(await sample.TaskAsync()).Should().Be("Hello!");
		(await sample.ValueTaskAsync()).Should().Be("Hello!");
	}

	[Test]
	public async Task TestAsyncEnumerable()
	{
		var sample = new CSharp10Features();

		await foreach (var s in sample.EnumerateAsync())
		{
			s.Should().Be("Hello!");
		}
	}
}