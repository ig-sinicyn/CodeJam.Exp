﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using static CodeJam.Targeting.TargetingFeatures;

namespace CodeJam.Targeting;

[PublicAPI]
public static class TargetingFeatures
{
	#region Diagnostics.Contracts

	[ContractsPure]
	public static int ContractsSample() => 42;

	#endregion

	#region System.ComponentModel.Annotations

	public enum SampleEnum
	{
		[Display(Name = "Sample Value")]
		[Required]
		SampleValue = 0
	}

	#endregion

	#region Tasks and async

	// ValueTasks are part of .Net Standard 2.1 or .Net Core 1.0 or later versions
	// We do reference System.Threading.Tasks.Extensions for other frameworks

	public static Task TaskSampleAsync(CancellationToken cancellation = default) => TaskEx.Delay(1, cancellation);

	public static Task<int> TaskOfTSampleAsync(CancellationToken cancellation = default) => TaskEx.Delay(1, cancellation).ContinueWith(_ => 42, cancellation);

	public static ValueTask ValueTaskSampleAsync(CancellationToken cancellation = default)
	{
		cancellation.ThrowIfCancellationRequested();
		return new();
	}

	public static ValueTask<int> ValueTaskOfTSampleAsync(CancellationToken cancellation = default)
	{
		cancellation.ThrowIfCancellationRequested();
		return new(42);
	}

	public static async Task AwaitTaskSampleAsync(CancellationToken cancellation = default) => await TaskSampleAsync(cancellation);

	public static async Task<int> AwaitTaskOfTSampleAsync(CancellationToken cancellation = default) => await TaskOfTSampleAsync(cancellation);

	public static async ValueTask AwaitValueTaskSampleAsync(CancellationToken cancellation = default) => await ValueTaskSampleAsync(cancellation);

	public static async ValueTask<int> AwaitValueTaskOfTSampleAsync(CancellationToken cancellation = default) => await ValueTaskOfTSampleAsync(cancellation);

	#endregion

	#region TaskCompletionSource

	public static Task<T> TaskCompletionResultSample<T>(T value)
	{
		var result = new TaskCompletionSource<T>();
		result.TrySetResult(value);
		return result.Task;
	}

	public static Task TaskCompletionCancellationSample(CancellationToken cancellation)
	{
		var result = new TaskCompletionSource<int>();
		result.TrySetCanceled(cancellation);
		return result.Task;
	}

	#endregion

	#region Records

#if NETCOREAPP20_OR_GREATER || NETSTANDARD20_OR_GREATER || NET40_OR_GREATER
	// No class records here, sorry.
	// https://github.com/dotnet/roslyn/issues/55812
	public record RecordSample(int Value, int Value2 = 42);
#endif

	public readonly record struct StructRecordSample(int Value, int Value2 = 42);

	#endregion

	#region Nullability

	public static T? NullabilitySample<T>(T input) => default;

	public static bool ConditionalNullabilitySample<T>([MaybeNullWhen(false)] out T result)
	{
		result = default!;
		return true;
	}

	[CollectionAccess(CollectionAccessType.Read)]
	public static TValue? GetValueSample<TKey, TValue>(
		this IReadOnlyDictionary<TKey, TValue> dictionary,
		TKey key,
		TValue? defaultValue)
			where TKey : notnull =>
		dictionary.TryGetValue(key, out var result)
			? result
			: defaultValue;

	public readonly record struct MemberNotNullWhenSample<T>(T? Value)
		where T : class
	{
		[MemberNotNullWhen(true, nameof(Value))]
		public bool HasValue => Value != null;
	}

	public static string MemberHasValueSample(string? input)
	{
		var result = new MemberNotNullWhenSample<string>(input);

		if (result.HasValue)
			return result.Value;

		return "N/A";
	}

	#endregion

	#region Init

	public class ClassWithInitSample
	{
		public string Property { get; init; } = default!;
	}

	#endregion

	#region System.HashCode()

#if NETCOREAPP21_OR_GREATER || NETSTANDARD21_OR_GREATER || FULL_TARGETING
	// System.HashCode is available as a part of
	// .Net Standard 2.1 or .Net Core 2.1 or later versions
	// We do not reference additional packages in lightweight targeting mode
	public static int HashCodeSample() => HashCode.Combine(1, 2);
#endif

	#endregion

	#region System.ValueTuple

	public static (int a, int b) ValueTupleSample()
	{
		var src = (1, 2);

		var (a, b) = src;

		return (a, b);
	}

	#endregion

	#region Deconstruct

	public static int DeconstructSample()
	{
		var src = new KeyValuePair<int, int>(1, 2);
		var (a, b) = src;

		return a + b;
	}

	#endregion

	#region System.Index

	public static int ArrayIndexSample()
	{
		var index = 1;
		return new[] { 1, 2, 3, 4 }[^index];
	}

	public static char StringIndexSample()
	{
		var index = 1;
		return "Hello!"[^index];
	}

	public static int ListIndexSample()
	{
		var index = 1;
		return new List<int> { 1, 2, 3, 4 }[^index];
	}

	public static int ArrayIndexTypeSample()
	{
		var index = Index.FromEnd(1);
		return new[] { 1, 2, 3, 4 }[index];
	}

	public static char StringIndexTypeSample()
	{
		var index = Index.FromEnd(1);
		return "Hello!"[index];
	}

	public static int ListIndexTypeSample()
	{
		var index = Index.FromEnd(1);
		return new List<int> { 1, 2, 3, 4 }[index];
	}

	#endregion

	#region System.Range

	public static int[] ArrayRangeSample()
	{
		var index = 1;
		return new[] { 1, 2, 3, 4 }[index..^index];
	}

	public static string StringRangeSample()
	{
		var index = 1;
		return "Hello!"[index..^index];
	}

	public static int[] ArrayRangeTypeSample()
	{
		var range = new Range(1, Index.FromEnd(1));
		return new[] { 1, 2, 3, 4 }[range];
	}

	public static string StringRangeTypeSample()
	{
		var range = new Range(1, Index.FromEnd(1));
		return "Hello!"[range];
	}

	#endregion

	#region Span

#if NETCOREAPP21_OR_GREATER || NETSTANDARD21_OR_GREATER || (FULL_TARGETING && (TARGETS_NETCOREAPP || NETSTANDARD11_OR_GREATER || NET45_OR_GREATER))
	// Spans are available as a part of
	// .Net Standard 2.1 or .Net Core 2.1 or later versions
	// We do not reference additional packages in lightweight targeting mode
	// In full targeting mode we do add span support via System.Memory reference
	public static int StackallocSpanSample()
	{
		Span<byte> data = stackalloc byte[] { 1, 2, 3, 4 };

		var subSpan = data[1..^1];

		ushort val = 0xABEF;

		MemoryMarshal.Write(subSpan, ref val);

		return MemoryMarshal.Read<int>(data);
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

	#region IAsyncEnumerable

#if NETCOREAPP30_OR_GREATER || NETSTANDARD21_OR_GREATER || (FULL_TARGETING && (TARGETS_NETCOREAPP || TARGETS_NETSTANDARD || NET45_OR_GREATER))
	// Async enumerables are available as a part of
	// .Net Standard 2.1 or .Net Core 3.0 or later versions
	// We do not reference additional packages in lightweight targeting mode
	// In full targeting mode we do add span support via Microsoft.Bcl.AsyncInterfaces reference
	public static async IAsyncEnumerable<int> EnumerateSampleAsync(
		[EnumeratorCancellation] CancellationToken cancellation = default)
	{
		await TaskEx.Yield();
		cancellation.ThrowIfCancellationRequested();
		yield return 42;
	}

	public static async ValueTask<int> UseAsyncEnumeratorAsync(CancellationToken cancellation = default)
	{
		await foreach (var v in EnumerateSampleAsync(cancellation))
		{
			return v;
		}

		throw new InvalidOperationException("Should not be thrown");
	}
#endif

	#endregion

	#region FormattableString

	public static string FormattableStringSample(FormattableString input) =>
		input.ToString(CultureInfo.InvariantCulture);

	public static string UseFormattableStringSample() =>
		FormattableStringSample($"Hello, {Environment.MachineName}");

	#endregion
}