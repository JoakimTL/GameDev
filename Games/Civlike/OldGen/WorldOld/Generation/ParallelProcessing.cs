//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.


//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace OldGen.WorldOld.Generation;

public static class ParallelProcessing {
	public static void ForFaces( GlobeModel globe, int reservedThreads, Action<int, int, int> parallelizedTask ) => For( (int) globe.FaceCount, reservedThreads, parallelizedTask );

	public static void For( int count, int reservedThreads, Action<int, int, int> parallelizedTask ) {
		int taskCount = Math.Max( Environment.ProcessorCount - reservedThreads, 1 );
		int facesPerTask = (int) Math.Ceiling( (double) count / taskCount );
		Task[] tasks = new Task[ taskCount ];

		for (int t = 0; t < taskCount; t++) {
			int start = t * facesPerTask;
			int end = Math.Min( start + facesPerTask, count );
			int taskId = t;
			tasks[ t ] = Task.Run( () => parallelizedTask( start, end, taskId ) );
		}
		Task.WaitAll( tasks );
	}
}
