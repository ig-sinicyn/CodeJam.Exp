using System.Threading.Tasks;

namespace CodeJam.Targeting.Demo;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal class CheckTargetingFeatures
{
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public record Demo(string A)
	{
		public string? NullablePropertyWithInit { get; init; } = "sample";
	}

	[Pure, ContractsPure]
	public async Task<string?> TaskAsync()
	{
		await Task.Yield();
		return "123"[2..];
	}

	public async ValueTask<string?> ValueTaskAsync()
	{
		await Task.Yield();
		return "123"[2..];
	}

	public async IAsyncEnumerable<string?> Z()
	{
		await Task.Yield();
		yield return "123"[2..];
	}
}