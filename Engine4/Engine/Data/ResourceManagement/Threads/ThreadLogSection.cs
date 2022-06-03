namespace Engine.Data.ResourceManagement.Threads;

public struct ThreadLogSection {

	public double Time { get; private set; }
	public ThreadState State { get; private set; }

	public ThreadLogSection( double time, ThreadState state ) {
		this.Time = time;
		this.State = state;
	}

	public override string ToString() => $"{this.Time:n2}:{this.State}";

}