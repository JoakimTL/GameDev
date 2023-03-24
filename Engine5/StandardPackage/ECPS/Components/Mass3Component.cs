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

    protected override string UniqueNameTag => $"{Mass}kg";

    public static Guid SerializationIdentity { get; } = new("4be73127-7168-4d6f-aef6-11265d5b83ed");
    public bool ShouldSerialize => true;

    public Mass3Component()
    {
        Mass = 1;
        InertiaTensor = Matrix4x4.Identity;
    }

    public void SetFromShape(IShape shape)
    {
        if (Mass == shape.Mass && InertiaTensor == shape.InertiaTensor)
            return;
        Mass = shape.Mass;
        InertiaTensor = shape.InertiaTensor;
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
                Mass = *(double*)srcPtr;
                InertiaTensor = *(Matrix4x4*)(srcPtr + sizeof(double));
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
                *(double*)dstPtr = Mass;
                *(Matrix4x4*)(dstPtr + sizeof(double)) = InertiaTensor;
            }
            return data;
        }
    }
}

//Get mass and inertia tensor from PhysicsShape3Component using a processor
