using Engine.Data;
using Engine.Data.Transforms;
using Engine.Modules.ECS;
using System.Numerics;

namespace Engine.Standard.ECS.Components;

/// <summary>
/// ComponentChanged trigger whenever the internal transform matrix changes.
/// </summary>
[Identifier( "0ec330e4-7587-400e-a805-8738a92fc082" )]
public class Transform3Component : TransformComponentBase<Vector3, Quaternion, Vector3> {

	private readonly Transform3 _transform;

	/// <summary>
	/// Takes effect when the entity's parent changes.
	/// </summary>
	public bool KeepWorldSpace { get; set; }

	public TransformInterface<Vector3, Quaternion, Vector3> Transform => this._transform.Interface;

	public Transform3Component() {
		this._transform = new();
		this._transform.MatrixChanged += OnMatrixChanged;
		EntitySet += OnEntitySet;
	}

	private void OnMatrixChanged( IMatrixProvider provider ) => TriggerChanged();

	private void OnEntitySet( ComponentBase component ) {
		this.Entity.AfterParentChange += OnParentChanged;
		this._transform.SetParent( this.Entity.Parent?.GetComponent<Transform3Component>()?._transform, this.KeepWorldSpace );
	}

	private void OnParentChanged( Entity? oldParent, Entity? newParent ) {
		this._transform.SetParent( this.Entity.Parent?.GetComponent<Transform3Component>()?._transform, this.KeepWorldSpace );
	}

	protected override TransformData<Vector3, Quaternion, Vector3> GetTransformData()
		=> this._transform.Data;

	protected override void SetFromTransformData( TransformData<Vector3, Quaternion, Vector3> data )
		=> this._transform.SetData( data );
}
