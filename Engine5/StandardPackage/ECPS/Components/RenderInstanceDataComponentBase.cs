using Engine;
using Engine.GameLogic.ECPS;
using Engine.Rendering;
using Engine.Structure.Attributes;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StandardPackage.ECPS.Components;

public abstract class RenderInstanceDataComponentBase<T> : ComponentBase, IRenderableInstanceData where T : unmanaged
{
    public Type? InstanceDataType { get; } = typeof(T);
    protected string _instanceTypeIdentity;
    protected byte[] InstanceData { get; set; }

    public RenderInstanceDataComponentBase()
    {
		this._instanceTypeIdentity = this.InstanceDataType.GetCustomAttribute<IdentityAttribute>()?.Identity ?? throw new ArgumentNullException($"No identity found for {typeof(T)}");
		this.InstanceData = new byte[Marshal.SizeOf<T>()];
    }

    protected void SetData(T data)
    {
        data.CopyInto( this.InstanceData );
        AlertComponentChanged();
    }

    public abstract ReadOnlySpan<byte> GetInstanceData(float time, out bool extrapolating);
}
