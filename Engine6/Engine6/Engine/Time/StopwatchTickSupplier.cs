using System.Diagnostics;

namespace Engine.Time;

public sealed class StopwatchTickSupplier : ITickSupplier {
	public static long Frequency => Stopwatch.Frequency;
	public static long Ticks => Stopwatch.GetTimestamp();
}
