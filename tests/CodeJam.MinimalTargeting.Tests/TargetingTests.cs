using FluentAssertions.Execution;

using System.ComponentModel.DataAnnotations;
using System.Reflection;

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
}