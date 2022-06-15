namespace Engine.Physics;
public class PhysicsShape<V> : ICollisionShape<V>, IPhysicsShape<V> where V : unmanaged {

	//Contains points with mass
	public V LocalCenterOfMass => throw new NotImplementedException();

	public V GetFurthestLocal( V direction ) => throw new NotImplementedException();
	public float GetIntertia( V localPoint, V force ) => throw new NotImplementedException();
	public V GetLocalPoint( int index ) => throw new NotImplementedException();
}

public interface IPhysicsShape<V> where V : unmanaged {

	V LocalCenterOfMass { get; }
	float GetIntertia( V localPoint, V force );

}

public interface ICollisionShape<V> where V : unmanaged {

	V LocalCenterOfMass { get; }

	V GetFurthestLocal( V direction );
	V GetLocalPoint( int index );
}


public interface ITransformedCollisionShape<V> : ICollisionShape<V> where V : unmanaged {

	V TransformedCenterOfMass { get; }

	V GetFurthestTransformed( V direction );
	V GetTransformedPoint( int index );
}
