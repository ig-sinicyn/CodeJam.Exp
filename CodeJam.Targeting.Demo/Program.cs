using CodeJam.Strings;

using System.Threading.Tasks;

namespace CodeJam.Targeting.Demo;

internal class Program
{
	public static async Task Main(string[] args)
	{
		await Task.Yield();

		var v = new[] { "a", "b" };

		Console.WriteLine(v.Join(";"));
	}
}
