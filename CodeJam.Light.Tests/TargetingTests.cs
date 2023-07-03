

#if NET45_OR_GREATER || TARGETS_NETCOREAPP
using TaskEx = System.Threading.Tasks.Task;
#elif NET40_OR_GREATER
using TaskEx = System.Threading.Tasks.TaskEx;
#else
using TaskEx = System.Threading.Tasks.Task;

#endif
using FluentAssertions;

// ReSharper disable once CheckNamespace
// ReSharper disable MethodSupportsCancellation


namespace CodeJam.Light.Tests;

[TestFixture]
public class TargetingTests
{
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
}