using System.Numerics;
using Engine.Data;
using Engine.Data.Datatypes.Transforms;
using Engine.Modularity.ECS.Networking;

namespace Engine.Modularity.ECS.Components;

[Identification( "b935417e-a3b9-4925-a106-6404cfcea528" )]
[Network( ComponentBroadcastSide.SERVER, System.Net.Sockets.ProtocolType.Udp )]
public class Transform3Component : TransformComponentBase<Vector3, Quaternion, Vector3> {
	public Transform3Component() : base( new Transform3() ) { }
}

public abstract class PhysicsSystemComponentBase<T, R> : SerializableComponent, IUpdateable where T : unmanaged where R : unmanaged {

	public float Mass { get; protected set; }
	public T LinearVelocity { get; protected set; }
	public R RotationalMomentum { get; protected set; }
	public bool Active { get; protected set; }

	public PhysicsSystemComponentBase() {
		this.Active = true;
	}

	public abstract void ApplyForce( T force, T relativePosition );

	public override void SetFromSerializedData( byte[] data ) {
		byte[][]? separated = Segmentation.Parse( data );
		if ( separated is null || separated.Length != 3 )
			return;
		this.Mass = DataUtils.ToUnmanaged<float>( separated[ 0 ] ) ?? default;
		this.LinearVelocity = DataUtils.ToUnmanaged<T>( separated[ 1 ] ) ?? default;
		this.RotationalMomentum = DataUtils.ToUnmanaged<R>( separated[ 2 ] ) ?? default;
	}

	public abstract void Update( float time, float deltaTime );
	protected override byte[]? GetSerializedData() => Segmentation.Segment( DataUtils.ToBytes( this.Mass ), DataUtils.ToBytes( this.LinearVelocity ), DataUtils.ToBytes( this.RotationalMomentum ) );
}

public class PhysicsSystemComponent3 : PhysicsSystemComponentBase<Vector3, Vector3> {

	//TODO: Allow component systems to listen for component list changes within their entity, and be active when all the component properties are available, and automatically deactivate when no longer available.

	public PhysicsSystemComponent3() {
	}

	public override void ApplyForce( Vector3 force, Vector3 relativePosition ) => throw new NotImplementedException();

	public override void Update( float time, float deltaTime ) {

	}
}
