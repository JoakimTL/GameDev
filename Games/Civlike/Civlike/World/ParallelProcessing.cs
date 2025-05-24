namespace Civlike.World;

public static class ParallelProcessing {

	public static int ReservedThreads { get; set; } = 2;
	public static int TaskCount => Math.Max( Environment.ProcessorCount - ReservedThreads, 1 );

	public static void Range( int count, Action<int, int, int> parallelizedTask ) {
		int taskCount = TaskCount;
		int numbersPerTask = (int) Math.Ceiling( (double) count / taskCount );
		Task[] tasks = new Task[ taskCount ];

		for (int t = 0; t < taskCount; t++) {
			int start = t * numbersPerTask;
			int end = Math.Min( start + numbersPerTask, count );
			int taskId = t;
			tasks[ t ] = Task.Run( () => parallelizedTask( start, end, taskId ) );
		}
		Task.WaitAll( tasks );
	}
}