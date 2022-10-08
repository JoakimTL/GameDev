namespace Engine.Modularity.ECS.Components;
public class TupleComponent<T1, T2> : Component where T1 : Component where T2 : Component {

	public T1? ComponentA { get; private set; }
	public T2? ComponentB { get; private set; }

	protected override void ParentSet() {
		this.ComponentA = this.Parent.GetComponent<T1>();
		this.ComponentB = this.Parent.GetComponent<T2>();
		this.Parent.ComponentAdded += ComponentAdded;
		this.Parent.ComponentRemoved += ComponentRemoved;
		if ( this.ComponentA is not null ) {
			this.ComponentA.Changed += ComponentsChanged;
			ComponentsChanged( this.ComponentA );
		}
		if ( this.ComponentB is not null ) {
			this.ComponentB.Changed += ComponentsChanged;
			ComponentsChanged( this.ComponentB );
		}
	}

	private void ComponentAdded( Entity e, Component c ) {
		if ( c is T1 t1 ) {
			this.ComponentA = t1;
			this.ComponentA.Changed += ComponentsChanged;
			ComponentsChanged( this.ComponentA );
		} else if ( c is T2 t2 ) {
			this.ComponentB = t2;
			this.ComponentB.Changed += ComponentsChanged;
			ComponentsChanged( this.ComponentB );
		}
	}

	private void ComponentRemoved( Entity e, Component c ) {
		if ( c is T1 ) {
			if ( this.ComponentA is not null )
				this.ComponentA.Changed -= ComponentsChanged;
			this.ComponentA = null;
			ComponentsChanged( this.ComponentA );
		} else if ( c is T2 ) {
			if ( this.ComponentB is not null )
				this.ComponentB.Changed -= ComponentsChanged;
			this.ComponentB = null;
			ComponentsChanged( this.ComponentB );
		}
	}

	private void ComponentsChanged( Component? c ) => TriggerChanged();
}
