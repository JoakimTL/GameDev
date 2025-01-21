using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;

namespace Engine.Module.Render.Ogl.Scenes;

public abstract class IndirectCommandProviderBase : DisposableIdentifiable, IComparable<IndirectCommandProviderBase> {
	public OglVertexArrayObjectBase VertexArrayObject { get; }
	public ShaderBundleBase ShaderBundle { get; }
	public ulong BindIndex { get; }
	public uint RenderLayer { get; }

	public event Action? OnChanged;

	protected IndirectCommandProviderBase( uint layer, OglVertexArrayObjectBase vertexArrayObject, ShaderBundleBase shaderBundle ) {
		this.RenderLayer = layer;
		this.VertexArrayObject = vertexArrayObject;
		this.ShaderBundle = shaderBundle;
		this.BindIndex = this.VertexArrayObject.GetBindIndexWith( this.ShaderBundle ) ?? throw new ArgumentException( "Unable to establish bind index" );
	}

	protected void InvokeChanged() => OnChanged?.Invoke();

	public int CompareTo( IndirectCommandProviderBase? other ) {
		if (other is null)
			return 1;
		return this.BindIndex.CompareTo( other.BindIndex );
	}

	public abstract void AddIndirectCommands( List<IndirectCommand> commandList );
}
