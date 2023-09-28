using Engine.Data;
using Engine.Data.Transforms;
using System.Numerics;

namespace Engine.Modules.Entity.Components;

/// <summary>
/// ComponentChanged trigger whenever the internal transform matrix changes.
/// </summary>
[Identifier( "2af60278-bc74-44af-8e01-de4954cffc21" )]
public class Transform2Component : TransformComponentBase<Vector2, float, Vector2> {

	private readonly Transform2 _transform;

	/// <summary>
	/// Takes effect when the entity's parent changes.
	/// </summary>
	public bool KeepWorldSpace { get; set; }

	public TransformInterface<Vector2, float, Vector2> Transform => this._transform.Interface;

	public Transform2Component() {
		this._transform = new();
		this._transform.MatrixChanged += OnMatrixChanged;
		EntitySet += OnEntitySet;
	}

	private void OnMatrixChanged( IMatrixProvider provider ) => TriggerChanged();

	private void OnEntitySet( ComponentBase component ) {
		this.Entity.AfterParentChange += OnParentChanged;
		this._transform.SetParent( this.Entity.Parent?.GetComponent<Transform2Component>()?._transform, this.KeepWorldSpace );
	}

	private void OnParentChanged( Entity? oldParent, Entity? newParent ) {
		this._transform.SetParent( this.Entity.Parent?.GetComponent<Transform2Component>()?._transform, this.KeepWorldSpace );
	}

	protected override TransformData<Vector2, float, Vector2> GetTransformData()
		=> this._transform.Data;

	protected override void SetFromTransformData( TransformData<Vector2, float, Vector2> data )
		=> this._transform.SetData( data );
}
