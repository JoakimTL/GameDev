using Engine.Data;
using Engine.Data.Transforms;
using Engine.Modules.ECS;
using System.Numerics;

namespace Engine.Standard.ECS.Components;

/// <summary>
/// ComponentChanged trigger whenever the internal transform matrix changes.
/// </summary>
[Identifier( "2af60278-bc74-44af-8e01-de4954cffc21" )]
public class Transform2Component : TransformComponentBase<double, Vector2<double>, double, Vector2<double>> {

	private readonly Transform2<double> _transform;

	/// <summary>
	/// Takes effect when the entity's parent changes.
	/// </summary>
	public bool KeepWorldSpace { get; set; }

	public override TransformInterface<double, Vector2<double>, double, Vector2<double>> Transform => this._transform.Interface;

	public Transform2Component() {
		this._transform = new();
		this._transform.MatrixChanged += OnMatrixChanged;
		EntitySet += OnEntitySet;
	}

	private void OnMatrixChanged( IMatrixProvider<double> provider ) => TriggerChanged();

	private void OnEntitySet( ComponentBase component ) {
		this.Entity.AfterParentChange += OnParentChanged;
		this._transform.SetParent( this.Entity.Parent?.GetComponent<Transform2Component>()?._transform, this.KeepWorldSpace );
	}

	private void OnParentChanged( Entity? oldParent, Entity? newParent ) {
		this._transform.SetParent( this.Entity.Parent?.GetComponent<Transform2Component>()?._transform, this.KeepWorldSpace );
	}

	protected override TransformData<Vector2<double>, double, Vector2<double>> GetTransformData()
		=> this._transform.Data;

	protected override void SetFromTransformData( TransformData<Vector2<double>, double, Vector2<double>> data )
		=> this._transform.SetData( data );
}
