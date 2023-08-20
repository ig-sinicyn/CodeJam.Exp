using FluentAssertions.Execution;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using static CodeJam.Targeting.TargetingFeatures;

namespace CodeJam.Targeting.Tests;

[TestFixture]
public class TargetingTests
{
	private static CancellationToken GetCanceledToken()
	{
		var cts = new CancellationTokenSource();
		cts.Cancel();
		return cts.Token;
	}

	private static readonly CancellationToken _canceledToken = GetCanceledToken();

	#region Diagnostics.Contracts

	[Test]
	public void ContractsAttribute_NotApplied()
	{
		// The PureAttribute is marked with [Conditional("CONTRACTS_FULL")] so it is not applied to the method.
		var method = typeof(TargetingFeatures).GetMethod(nameof(ContractsSample));

		var contractsAttribute = method!.GetCustomAttribute<ContractsPureAttribute>();

		contractsAttribute.Should().BeNull();
	}

	#endregion

	#region System.ComponentModel.Annotations

	[Test]
	public void DisplayAndRequiredAttributes_Applied()
	{
		var enumField = typeof(TargetingFeatures.SampleEnum)!.GetField(nameof(TargetingFeatures.SampleEnum.SampleValue));

		var displayAttribute = enumField?.GetCustomAttribute<DisplayAttribute>();
		var requiredAttribute = enumField?.GetCustomAttribute<RequiredAttribute>();

		using (new AssertionScope())
		{
			displayAttribute.Should().NotBeNull();
			requiredAttribute.Should().NotBeNull();
		}
	}

	public enum SampleEnum
	{
		[Display(Name = "Sample Value")]
		[Required]
		SampleValue = 0
	}

	#endregion

	#region Tasks and async

	[Test]
#if LESSTHAN_NET45
	public void Task_OkShim() => Task_Ok().Wait();
#endif
	public async Task Task_Ok() => await TaskSampleAsync();

	[Test]
#if LESSTHAN_NET45
	public void TaskOfT_OkShim() => TaskOfT_Ok().Wait();
#endif
	public async Task TaskOfT_Ok()
	{
		var result = await TaskOfTSampleAsync();
		result.Should().Be(42);
	}

	[Test]
#if LESSTHAN_NET45
	public void ValueTask_OkShim() => ValueTask_Ok().Wait();
#endif
	public async Task ValueTask_Ok() => await ValueTaskSampleAsync();

	[Test]
#if LESSTHAN_NET45
	public void ValueTaskOfT_OkShim() => ValueTaskOfT_Ok().Wait();
#endif
	public async Task ValueTaskOfT_Ok()
	{
		var result = await ValueTaskOfTSampleAsync();
		result.Should().Be(42);
	}

	[Test]
#if LESSTHAN_NET45
	public void AwaitTask_OkShim() => AwaitTask_Ok().Wait();
#endif
	public async Task AwaitTask_Ok() => await AwaitTaskSampleAsync();

	[Test]
#if LESSTHAN_NET45
	public void AwaitTaskOfT_OkShim() => AwaitTaskOfT_Ok().Wait();
#endif
	public async Task AwaitTaskOfT_Ok()
	{
		var result = await AwaitTaskOfTSampleAsync();
		result.Should().Be(42);
	}

	[Test]
#if LESSTHAN_NET45
	public void AwaitValueTask_OkShim() => AwaitValueTask_Ok().Wait();
#endif
	public async Task AwaitValueTask_Ok() => await AwaitValueTaskSampleAsync();

	[Test]
#if LESSTHAN_NET45
	public void AwaitValueTaskOfT_OkShim() => AwaitValueTaskOfT_Ok().Wait();
#endif
	public async Task AwaitValueTaskOfT_Ok()
	{
		var result = await AwaitValueTaskOfTSampleAsync();
		result.Should().Be(42);
	}

	#endregion

	#region Tasks and async (cancellation)

	[Test]
	public void AwaitTask_Canceled_Throws() =>
		AssertEx.ThrowsAsync<TaskCanceledException>(() =>
			AwaitTaskSampleAsync(_canceledToken));
	[Test]
	public void AwaitTaskOfT_Canceled_Throws() =>
		AssertEx.ThrowsAsync<TaskCanceledException>(() =>
			AwaitTaskOfTSampleAsync(_canceledToken));

	[Test]
	public void AwaitValueTask_Canceled_Throws() =>
#if LESSTHAN_NET45
		AssertEx.ThrowsAsync<TaskCanceledException>(async () =>
#else
		AssertEx.ThrowsAsync<OperationCanceledException>(async () =>
#endif
			await AwaitValueTaskSampleAsync(_canceledToken));

	[Test]
	public void AwaitValueTaskOfT_Canceled_Throws() =>
#if LESSTHAN_NET45
		AssertEx.ThrowsAsync<TaskCanceledException>(async () =>
#else
		AssertEx.ThrowsAsync<OperationCanceledException>(async () =>
#endif
		await AwaitValueTaskOfTSampleAsync(_canceledToken));

	#endregion

	#region Records

#if NETCOREAPP20_OR_GREATER || NETSTANDARD20_OR_GREATER || NET40_OR_GREATER
	[Test]
	public void Record_Ok()
	{
		var record = new RecordSample(1);
		record.Value.Should().Be(1);

		var other = record with { Value2 = 1 };
		other.Value2.Should().Be(1);
	}
#endif

	[Test]
	public void StructRecord_Ok()
	{
		var record = new StructRecordSample(1);
		record.Value.Should().Be(1);

		var other = record with { Value2 = 1 };
		other.Value2.Should().Be(1);
	}

	#endregion

	#region Nullability

	[Test]
	public void NullabilityAnnotations_Ok()
	{
		var method = typeof(TargetingFeatures).GetMethod(nameof(NullabilitySample))!;
		var nullableAttributeTypes = method.GetCustomAttributes()
			.Select(x => x.GetType())
			.Where(x => x.Namespace == "System.Runtime.CompilerServices"
				&& x.Name.StartsWith("NullableContext"));

		nullableAttributeTypes.Should().HaveCount(1);
	}

	#endregion

	#region Init

	[Test]
	public void InitUsage_Ok()
	{
		var sample = new ClassWithInitSample { Property = "Hello!" };

		sample.Property.Should().Be("Hello!");
	}

	#endregion

	#region System.HashCode()

#if NETCOREAPP21_OR_GREATER || NETSTANDARD21_OR_GREATER || FULL_TARGETING
	[Test]
	public void HashCode_Ok() => HashCodeSample().Should().Be(HashCode.Combine(1, 2));
#endif

	#endregion

	#region System.ValueTuple

	[Test]
	public void ValueTupleCode_Ok() => ValueTupleSample().a.Should().Be(1);

	#endregion

	#region Deconstruct

	[Test]
	public void Deconstruct_Ok() => DeconstructSample().Should().Be(3);

	#endregion

	#region System.Index

	[Test]
	public void ArrayIndex_Ok() => ArrayIndexSample().Should().Be(4);

	[Test]
	public void StringIndex_Ok() => StringIndexSample().Should().Be('!');

	[Test]
	public void ListIndex_Ok() => ListIndexSample().Should().Be(4);

	[Test]
	public void ArrayIndexType_Ok() => ArrayIndexTypeSample().Should().Be(4);

	[Test]
	public void StringIndexType_Ok() => StringIndexTypeSample().Should().Be('!');

	[Test]
	public void ListIndexType_Ok() => ListIndexTypeSample().Should().Be(4);

	#endregion

	#region System.Range

	[Test]
	public void ArrayRange_Ok() => ArrayRangeSample().Should().BeEquivalentTo(new[] { 2, 3 });

	[Test]
	public void StringRange_Ok() => StringRangeSample().Should().Be("ello");

	[Test]
	public void ArrayRangeType_Ok() => ArrayRangeTypeSample().Should().BeEquivalentTo(new[] { 2, 3 });

	[Test]
	public void StringRangeType_Ok() => StringRangeTypeSample().Should().Be("ello");

	#endregion

	#region Span

#if (NETCOREAPP21_OR_GREATER || NETSTANDARD21_OR_GREATER) || (FULL_TARGETING && (TARGETS_NETCOREAPP || NETSTANDARD11_OR_GREATER || NET45_OR_GREATER))
	[Test]
	public void StackallocSpan_Ok() => StackallocSpanSample().Should().Be(0x04ABEF01);

	[Test]
	public void ArraySpan_Ok() => ArraySpanSample().Should().Be(1);

	[Test]
	public void StringSpan_Ok() => StringSpanSample().Should().Be('H');
#endif

	#endregion

	#region IAsyncEnumerable

#if NETCOREAPP30_OR_GREATER || NETSTANDARD21_OR_GREATER || (FULL_TARGETING && (TARGETS_NETCOREAPP || TARGETS_NETSTANDARD || NET45_OR_GREATER))
	[Test]
	public async Task AsyncForeach_Ok()
	{
		var count = 0;
		await foreach (var x in EnumerateSampleAsync())
		{
			x.Should().Be(42);
			count++;
		}
		count.Should().Be(1);
	}

	[Test]
	public async Task UseAsyncEnumerator_Ok()
	{
		var result = await UseAsyncEnumeratorAsync();
		result.Should().Be(42);
	}
#endif

	#endregion

	#region IAsyncEnumerable (cancellation)

#if NETCOREAPP30_OR_GREATER || NETSTANDARD21_OR_GREATER || (FULL_TARGETING && (TARGETS_NETCOREAPP || TARGETS_NETSTANDARD || NET45_OR_GREATER))
	[Test]
	public void AsyncForeach_Canceled_Throws() =>
		AssertEx.ThrowsAsync<OperationCanceledException>(async () =>
		{
			await foreach (var _ in EnumerateSampleAsync(_canceledToken))
			{
			}
		});

	[Test]
	public void UseAsyncEnumerator_Canceled_Throws() =>
		AssertEx.ThrowsAsync<OperationCanceledException>(async () =>
			await UseAsyncEnumeratorAsync(_canceledToken));
#endif

	#endregion

}