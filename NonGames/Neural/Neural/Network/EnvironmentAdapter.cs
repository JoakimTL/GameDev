
using System.Collections.Concurrent;

namespace Neural.Network;

// ------------------------------------------------------
// 6) Minimal host environment & a runnable demonstration
// ------------------------------------------------------
public sealed class EnvironmentAdapter {
	private readonly ConcurrentQueue<char> _in = new();
	public void Enqueue( string s ) { foreach (var c in s) _in.Enqueue( c ); }

	public bool TryReadNext( out char c ) => _in.TryDequeue( out c );

	// Replace with your true task objective:
	// Here: reward = 1 if user types 'y' after emission, else 0.
	public float EvaluateInteractive( string emitted ) {
		Console.WriteLine();
		Console.WriteLine( $"NET EMITTED: \"{emitted}\"  Approve? (y/n)" );
		while (true) {
			var k = Console.ReadKey( true ).KeyChar;
			if (k == 'y' || k == 'Y')
				return 1f;
			if (k == 'n' || k == 'N')
				return 0f;
		}
	}
}
