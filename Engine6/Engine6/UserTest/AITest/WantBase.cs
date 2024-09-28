using Engine.Reflection;

namespace UserTest.AITest;

public abstract class WantBase( string name ) {

	private static readonly List<WantBase> _wants = [];
	public WantBase Get( int id ) => _wants[ id ];

	static WantBase() {
		foreach (Type t in TypeUtilities.GetAllSubtypes<WantBase>()) {
			if (!t.TryInstantiate( out WantBase? want ))
				continue;
			want.Id = _wants.Count;
			_wants.Add( want );
		}
	}

	public string Name { get; } = name;
	public int Id { get; private set; }

	public abstract double DeterminePriority( AgentBase agent );
}
