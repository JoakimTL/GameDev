using System.Diagnostics;

namespace Engine.Data;
public static class GuidGenerator {

	private static long _generatedGuids = Random.Shared.NextInt64();

	public static Guid GenerateGuid() {
		long part1 = Stopwatch.GetTimestamp();
		long part2 = Interlocked.Increment( ref _generatedGuids );
		unsafe {
			byte* data = stackalloc byte[ 16 ];
			*(long*) data = part1;
			*(long*) ( data + 8 ) = part2;
			return *(Guid*) data;
		}
	}
}
