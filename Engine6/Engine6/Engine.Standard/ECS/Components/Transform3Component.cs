using Engine.Data;
using Engine.Data.Transforms;
using Engine.Modules.ECS;
using System.Numerics;

namespace Engine.Standard.ECS.Components;

/// <summary>
/// ComponentChanged trigger whenever the internal transform matrix changes.
/// </summary>
[Identifier( "0ec330e4-7587-400e-a805-8738a92fc082" )]
public class Transform3Component : TransformComponentBase<double, Vector3<double>, Rotor3<double>, Vector3<double>> {

	private readonly Transform3<double> _transform;

	/// <summary>
	/// Takes effect when the entity's parent changes.
	/// </summary>
	public bool KeepWorldSpace { get; set; }

	public override TransformInterface<double, Vector3<double>, Rotor3<double>, Vector3<double>> Transform => this._transform.Interface;

	public Transform3Component() {
		this._transform = new();
		this._transform.MatrixChanged += OnMatrixChanged;
		EntitySet += OnEntitySet;
	}

	private void OnMatrixChanged( IMatrixProvider<double> provider ) => TriggerChanged();

	private void OnEntitySet( ComponentBase component ) {
		this.Entity.AfterParentChange += OnParentChanged;
		this._transform.SetParent( this.Entity.Parent?.GetComponent<Transform3Component>()?._transform, this.KeepWorldSpace );
	}

	private void OnParentChanged( Entity? oldParent, Entity? newParent ) {
		this._transform.SetParent( this.Entity.Parent?.GetComponent<Transform3Component>()?._transform, this.KeepWorldSpace );
	}

	protected override TransformData<Vector3<double>, Rotor3<double>, Vector3<double>> GetTransformData()
		=> this._transform.Data;

	protected override void SetFromTransformData( TransformData<Vector3<double>, Rotor3<double>, Vector3<double>> data )
		=> this._transform.SetData( data );
}
