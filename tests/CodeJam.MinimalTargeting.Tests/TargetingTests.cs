using FluentAssertions.Execution;

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeJam.Targeting.Tests;

[TestFixture]
public class TargetingTests
{
	private static CancellationToken GetCancelledToken()
	{
		var cts = new CancellationTokenSource();
		cts.Cancel();
		return cts.Token;
	}

	private static readonly CancellationToken _cancelledToken = GetCancelledToken();

	#region Diagnostics.Contracts

	[Test]
	public void ContractsAttribute_NotApplied()
	{
		// The PureAttribute is marked with [Conditional("CONTRACTS_FULL")] so it is not applied to the method.
		var method = typeof(TargetingFeatures).GetMethod(nameof(TargetingFeatures.ContractsSample));

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
	public async Task Task_Ok() => await TargetingFeatures.TaskSampleAsync();

	[Test]
#if LESSTHAN_NET45
	public void TaskOfT_OkShim() => TaskOfT_Ok().Wait();
#endif
	public async Task TaskOfT_Ok()
	{
		var result = await TargetingFeatures.TaskOfTSampleAsync();
		result.Should().Be(42);
	}

	[Test]
#if LESSTHAN_NET45
	public void ValueTask_OkShim() => ValueTask_Ok().Wait();
#endif
	public async Task ValueTask_Ok() => await TargetingFeatures.ValueTaskSampleAsync();

	[Test]
#if LESSTHAN_NET45
	public void ValueTaskOfT_OkShim() => ValueTaskOfT_Ok().Wait();
#endif
	public async Task ValueTaskOfT_Ok()
	{
		var result = await TargetingFeatures.ValueTaskOfTSampleAsync();
		result.Should().Be(42);
	}

	[Test]
#if LESSTHAN_NET45
	public void AwaitTask_OkShim() => AwaitTask_Ok().Wait();
#endif
	public async Task AwaitTask_Ok() => await TargetingFeatures.AwaitTaskSampleAsync();

	[Test]
#if LESSTHAN_NET45
	public void AwaitTaskOfT_OkShim() => AwaitTaskOfT_Ok().Wait();
#endif
	public async Task AwaitTaskOfT_Ok()
	{
		var result = await TargetingFeatures.AwaitTaskOfTSampleAsync();
		result.Should().Be(42);
	}

	[Test]
#if LESSTHAN_NET45
	public void AwaitValueTask_OkShim() => AwaitValueTask_Ok().Wait();
#endif
	public async Task AwaitValueTask_Ok() => await TargetingFeatures.AwaitValueTaskSampleAsync();

	[Test]
#if LESSTHAN_NET45
	public void AwaitValueTaskOfT_OkShim() => AwaitValueTaskOfT_Ok().Wait();
#endif
	public async Task AwaitValueTaskOfT_Ok()
	{
		var result = await TargetingFeatures.AwaitValueTaskOfTSampleAsync();
		result.Should().Be(42);
	}

	#endregion

	#region Tasks and async (cancellation)

	[Test]
	public void AwaitTask_Canceled_Throws() =>
		AssertEx.ThrowsAsync<TaskCanceledException>(() =>
			TargetingFeatures.AwaitTaskSampleAsync(_cancelledToken));
	[Test]
	public void AwaitTaskOfT_Canceled_Throws() =>
		AssertEx.ThrowsAsync<TaskCanceledException>(() =>
			TargetingFeatures.AwaitTaskOfTSampleAsync(_cancelledToken));

	[Test]
	public void AwaitValueTask_Canceled_Throws() =>
#if LESSTHAN_NET45
		AssertEx.ThrowsAsync<TaskCanceledException>(async () =>
#else
		AssertEx.ThrowsAsync<OperationCanceledException>(async () =>
#endif
			await TargetingFeatures.AwaitValueTaskSampleAsync(_cancelledToken));

	[Test]
	public void AwaitValueTaskOfT_Canceled_Throws() =>
#if LESSTHAN_NET45
		AssertEx.ThrowsAsync<TaskCanceledException>(async () =>
#else
		AssertEx.ThrowsAsync<OperationCanceledException>(async () =>
#endif
		await TargetingFeatures.AwaitValueTaskOfTSampleAsync(_cancelledToken));

	#endregion
}