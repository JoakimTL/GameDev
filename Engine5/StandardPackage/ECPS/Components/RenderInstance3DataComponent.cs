using Engine;
using Engine.Datatypes.Colors;
using Engine.Datatypes.Transforms;
using Engine.Structure.Interfaces;
using StandardPackage.Rendering.VertexArrayLayouts;
using System.Numerics;

namespace StandardPackage.ECPS.Components;

public sealed class RenderInstance3DataComponent : RenderInstanceDataComponentBase<Entity3SceneData>, ICustomizedSerializable {
	public static Guid SerializationIdentity { get; } = new( "fc52578f-949c-464b-86ec-80bbb2a14805" );
	public bool ShouldSerialize => true;

	private float _previousTime;
	private TransformData<Vector3, Quaternion, Vector3> _previousTransform;
	private float _currentTime;
	private TransformData<Vector3, Quaternion, Vector3> _currentTransform;
	private TransformData<Vector3, Quaternion, Vector3> _extrapolatedTargetTransform;
	private TransformData<Vector3, Quaternion, Vector3> _previousDisplayedTransform;
	private TransformData<Vector3, Quaternion, Vector3> _displayedTransform;

	private Color16x4 _color;
	private bool _normalMapped;

	private bool _updated;
	private bool _hasFirstUpdate;

	public override ReadOnlySpan<byte> GetInstanceData( float time, out bool extrapolating ) {

		if ( this._updated ) {
			this._updated = false;
			this._previousTime = this._currentTime;
			this._currentTime = time;
			this._previousDisplayedTransform = this._displayedTransform;
			if ( !this._hasFirstUpdate ) {
				this._hasFirstUpdate = true;
				this._previousTime = this._currentTime;
				this._previousTransform = this._currentTransform;
				this._displayedTransform = this._currentTransform;
				this._previousDisplayedTransform = this._currentTransform;
				this._extrapolatedTargetTransform = this._previousTransform.GetInterpolated( this._currentTransform, 2 );
			}
		}
		extrapolating = this._currentTime - this._previousTime > 0;
		if ( extrapolating ) {
			float interpolationFactor = ( time - this._currentTime ) / ( this._currentTime - this._previousTime );
			if ( interpolationFactor > 2 ) {
				extrapolating = false;
				return this.InstanceData;
			}
			if ( interpolationFactor <= 1 ) {
				this._displayedTransform = this._previousDisplayedTransform.GetInterpolated( _extrapolatedTargetTransform, interpolationFactor );
			} else {
				this._displayedTransform = this._displayedTransform.GetInterpolated( _currentTransform, interpolationFactor - 1 );
			}
			new Entity3SceneData() {
				ModelMatrix = this._displayedTransform.GetMatrix(),
				Color = _color,
				NormalMapped = this._normalMapped ? byte.MaxValue : byte.MinValue
			}.CopyInto( this.InstanceData );
		}
		return this.InstanceData;
	}

	public void Set( TransformData<Vector3, Quaternion, Vector3> transform, Color16x4 color, bool normalMapped = true ) {
		this._previousTransform = this._currentTransform;
		this._currentTransform = transform;
		this._extrapolatedTargetTransform = this._previousTransform.GetInterpolated( this._currentTransform, 2 );
		this._color = color;
		this._normalMapped = normalMapped;
		this._updated = true;
		AlertComponentChanged();
	}

	public bool DeserializeData( byte[] data ) {
		if ( data.Length != this.InstanceData.Length )
			return false;
		this.InstanceData = data;
		AlertComponentChanged();
		return true;
	}

	public byte[] SerializeData() => this.InstanceData;
}