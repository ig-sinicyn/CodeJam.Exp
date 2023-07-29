using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CodeJam.MinimalTargeting.Example;

[PublicAPI]
public static class TargetingFeatures
{
	#region Diagnostics.Contracts

	[System.Diagnostics.Contracts.Pure]
	public static int ContractsSample() => 42;

	#endregion

	#region Tasks and async

	public static Task TaskSampleAsync(CancellationToken cancellation) => TaskEx.Delay(1, cancellation);

	public static Task<int> TaskOfTSampleAsync(CancellationToken cancellation) => TaskEx.FromResult(42);

	public static ValueTask ValueTaskSampleAsync(CancellationToken cancellation) => new();

	public static ValueTask<int> ValueTaskOfTSampleAsync(CancellationToken cancellation) => new(42);

	public static async Task AwaitTaskSampleAsync(CancellationToken cancellation)
	{
		await TaskEx.Delay(1, cancellation);
		await TaskSampleAsync(cancellation);
	}

	public static async Task<int> AwaitTaskOfTSampleAsync(CancellationToken cancellation)
	{
		await TaskEx.Delay(1, cancellation);
		return await TaskOfTSampleAsync(cancellation);
	}

	public static async ValueTask AwaitValueTaskSampleAsync(CancellationToken cancellation)
	{
		await TaskEx.Delay(1, cancellation);
		await ValueTaskSampleAsync(cancellation);
	}

	public static async ValueTask<int> AwaitValueTaskOfTSampleAsync(CancellationToken cancellation)
	{
		await TaskEx.Delay(1, cancellation);
		return await ValueTaskOfTSampleAsync(cancellation);
	}

	#endregion

	#region Records

#if NET40_OR_GREATER || NETSTANDARD16_OR_GREATER || NETCOREAPP20_OR_GREATER
	// No class records here, sorry.
	// https://github.com/dotnet/roslyn/issues/55812
	public record RecordSample(int Value = 42);
#endif

	public record struct StructRecordSample(int Value = 42);

	#endregion

	#region Nullability

	public static T? NullabilitySample<T>(T input) => default;

	public static bool ConditionalNullabilitySample<T>([MaybeNullWhen(false)] out T result)
	{
		result = default!;
		return true;
	}

	#endregion

	#region Init

	public class ClassWithInitSample
	{
		public string Property { get; init; } = default!;
	}

	#endregion

	#region IAsyncEnumerable

#if NET45_OR_GREATER || TARGETS_NETSTANDARD || TARGETS_NETCOREAPP
	public static async IAsyncEnumerable<int> EnumerateSampleAsync(
		[EnumeratorCancellation] CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		await TaskEx.Yield();
		yield return 42;
	}

	public static async ValueTask<int> UseAsyncEnumeratorAsync(CancellationToken cancellation)
	{
		await foreach (var v in EnumerateSampleAsync(cancellation))
		{
			return v;
		}

		throw new InvalidOperationException("Should not be thrown");
	}
#endif

	#endregion

	#region Index

#if TARGETS_NETCOREAPP

	public static int[] ArraySliceSample()
	{
		var s = new Index(1);
		var e = Index.FromEnd(1);
		var r = new Range(s, e);
		return new[] { 1, 2, 3, 4 }[r];
	}

	public static int[] ArraySliceSyntaxSample() => new[] { 1, 2, 3, 4 }[1..^1];

	public static string StringSliceSample()
	{
		var s = new Index(1);
		var e = Index.FromEnd(1);
		var r = new Range(s, e);
		return "Hello!"[r];
	}

	public static string StringSliceSyntaxSample() => "Hello!"[1..^1];
#endif

	#endregion

	#region Span

#if TARGETS_NETCOREAPP
	public static int StackallocSpanSample()
	{
		Span<byte> data = stackalloc byte[] { 1, 2, 3, 4 };

		var subSpan = data[1..^1];

		var val = 0xEFu;

		MemoryMarshal.Write(subSpan, ref val);

		return data[1];
	}


	public static int ArraySpanSample()
	{
		ReadOnlySpan<int> data = new[] { 1, 2, 3, 4 }.AsSpan();

		return data[0];
	}

	public static char StringSpanSample()
	{
		var data = "Hello!".AsSpan();

		return data[0];
	}
#endif

	#endregion
}