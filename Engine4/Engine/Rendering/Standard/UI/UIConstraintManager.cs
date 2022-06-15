using System.Numerics;
using Engine.Data.Datatypes.Transforms;

namespace Engine.Rendering.Standard.UI;

public class UIConstraintManager<T> : IUIConstraint<T> where T : class{

	private readonly List<IUIConstraint<T>> _constraints;
	private bool _newConstraints;

	public int ExecutionOrder { get; }

	public UIConstraintManager( int executionOrder = 0 ) {
		this.ExecutionOrder = executionOrder;
		this._constraints = new List<IUIConstraint<T>>();
	}

	public void Apply( float time, float deltaTime, T data ) {
		if ( this._constraints.Count == 0 )
			return;

		if ( this._newConstraints ) {
			this._newConstraints = false;
			this._constraints.Sort( ConstraintSorter );
		}

		for ( int i = 0; i < this._constraints.Count; i++ )
			this._constraints[ i ].Apply( time, deltaTime, data );
	}

	public void AddConstraint( IUIConstraint<T> constraint ) {
		this._constraints.Add( constraint );
		this._newConstraints = true;
	}

	public void RemoveConstraint( IUIConstraint<T> constraint ) => this._constraints.Remove( constraint );

	private int ConstraintSorter( IUIConstraint<T> x, IUIConstraint<T> y ) => x.ExecutionOrder - y.ExecutionOrder;
}