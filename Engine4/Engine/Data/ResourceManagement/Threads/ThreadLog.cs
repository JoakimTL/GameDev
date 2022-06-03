namespace Engine.Data.ResourceManagement.Threads;

public class ThreadLog {

	private readonly List<ThreadLogSection> _sections;

	public ThreadLogSection Latest {
		get {
			if ( this._sections.Count > 0 )
				return this._sections[ ^1 ];
			return new ThreadLogSection( -1, ThreadState.Unstarted );
		}
	}

	public IReadOnlyList<ThreadLogSection> Sections => this._sections;

	public ThreadLog() {
		this._sections = new List<ThreadLogSection>();
	}

	public void Append( ThreadLogSection section ) => this._sections.Add( section );

}
