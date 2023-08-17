using FluentAssertions.Execution;

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeJam.Targeting.Tests;

[TestFixture]
public class TargetingTests
{
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
	public void TaskAwait_OkShim() => TaskAwait_Ok().Wait();
#endif
	public async Task TaskAwait_Ok() => await TargetingFeatures.TaskSampleAsync();

	// ValueTasks are part of .Net Standard 2.1 or .Net Core 1.0 or later versions
	// We do reference System.Threading.Tasks.Extensions for other frameworks

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
}