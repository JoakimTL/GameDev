using Engine;
using Engine.GameLogic.ECPS;
using Engine.Physics;
using Engine.Structure.Interfaces;
using System.Numerics;

namespace StandardPackage.ECPS.Components;

public sealed class Mass3Component : ComponentBase, ICustomizedSerializable
{
    public double Mass { get; set; }
    public Matrix4x4 InertiaTensor { get; set; }

    protected override string UniqueNameTag => $"{this.Mass}kg";

    public static Guid SerializationIdentity { get; } = new("4be73127-7168-4d6f-aef6-11265d5b83ed");
    public bool ShouldSerialize => true;

    public Mass3Component()
    {
		this.Mass = 1;
		this.InertiaTensor = Matrix4x4.Identity;
    }

    public void SetFromShape(IShape shape)
    {
        if ( this.Mass == shape.Mass && this.InertiaTensor == shape.InertiaTensor)
            return;
		this.Mass = shape.Mass;
		this.InertiaTensor = shape.InertiaTensor;
        AlertComponentChanged();
    }

    public bool DeserializeData(byte[] data)
    {
        unsafe
        {
            if ((uint)data.Length < sizeof(double) + sizeof(Matrix4x4))
                return Log.WarningThenReturn($"Length of data must be equal to or longer than {sizeof(Vector3)}", false);
            fixed (byte* srcPtr = data)
            {
				this.Mass = *(double*)srcPtr;
				this.InertiaTensor = *(Matrix4x4*)(srcPtr + sizeof(double));
            }
        }
        return true;
    }

    public byte[] SerializeData()
    {
        unsafe
        {
            byte[] data = new byte[sizeof(double) + sizeof(Matrix4x4)];
            fixed (byte* dstPtr = data)
            {
                *(double*)dstPtr = this.Mass;
                *(Matrix4x4*)(dstPtr + sizeof(double)) = this.InertiaTensor;
            }
            return data;
        }
    }
}

//Get mass and inertia tensor from PhysicsShape3Component using a processor
