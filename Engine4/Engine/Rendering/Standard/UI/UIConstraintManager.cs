using System.Numerics;
using Engine.Data.Datatypes.Transforms;

namespace Engine.Rendering.Standard.UI;

public class UIConstraintManager : IUIConstraint {

	private readonly List<IUIConstraint> _constraints;
	private bool _newConstraints;

	public int ExecutionOrder { get; }

	public UIConstraintManager( int executionOrder = 0 ) {
		this.ExecutionOrder = executionOrder;
		this._constraints = new List<IUIConstraint>();
	}

	public void Apply( float time, float deltaTime, Transform2 transform ) {
		if ( this._constraints.Count == 0 )
			return;

		if ( this._newConstraints ) {
			this._newConstraints = false;
			this._constraints.Sort( ConstraintSorter );
		}

		transform.SetData( new TransformData<Vector2, float, Vector2>( Vector2.Zero, 0, Vector2.One ) );

		for ( int i = 0; i < this._constraints.Count; i++ )
			this._constraints[ i ].Apply( time, deltaTime, transform );
	}

	public void AddConstraint( IUIConstraint constraint ) {
		this._constraints.Add( constraint );
		this._newConstraints = true;
	}

	public void RemoveConstraint( IUIConstraint constraint ) => this._constraints.Remove( constraint );

	private int ConstraintSorter( IUIConstraint x, IUIConstraint y ) => x.ExecutionOrder - y.ExecutionOrder;
}