using System.Collections.Concurrent;

namespace OldGen.World;

public static class ParallelProcessing {

	public static int ReservedThreads { get; set; } = 2;
	public static int TaskCount => Math.Max( Environment.ProcessorCount - ReservedThreads, 1 );
	private static Task[] _tasks = Array.Empty<Task>();

	public static void Range( int count, Action<int, int, int> parallelizedTask ) {
		//int taskCount = TaskCount;
		//if (_tasks.Length != TaskCount)
		//	_tasks = new Task[ taskCount ];
		//int numbersPerTask = (int) Math.Ceiling( (double) count / taskCount );

		//for (int t = 0; t < taskCount; t++) {
		//	int start = t * numbersPerTask;
		//	int end = Math.Min( start + numbersPerTask, count );
		//	int taskId = t;
		//	_tasks[ t ] = Task.Run( () => parallelizedTask( start, end, taskId ) );
		//}
		//Task.WaitAll( _tasks );
		int maxDop = Math.Max( Environment.ProcessorCount - ReservedThreads, 1 );
		ParallelOptions options = new() { MaxDegreeOfParallelism = maxDop };

		OrderablePartitioner<Tuple<int, int>> partitions = Partitioner.Create( 0, count, (int) Math.Ceiling( count / (double) maxDop ) );
		int taskId = 0;
		Parallel.ForEach( partitions, options, () => Interlocked.Increment( ref taskId ) - 1,
			( range, loopState, id ) => {
				parallelizedTask( range.Item1, range.Item2, id );
				return id;
			},
			_ => { } );
	}
}