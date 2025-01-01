using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Providers;

namespace Engine.Standard.Render.UserInterface;

public sealed class UserInterfaceServiceAccess( RenderServiceAccess renderServiceAccess ) {
	public SceneInstanceProvider SceneInstanceProvider { get; } = renderServiceAccess.Get<SceneInstanceProvider>();
	public CompositeVertexArrayProvider CompositeVertexArrayProvider { get; } = renderServiceAccess.Get<CompositeVertexArrayProvider>();
	public ShaderBundleProvider ShaderBundleProvider { get; } = renderServiceAccess.Get<ShaderBundleProvider>();
	public MeshProvider MeshProvider { get; } = renderServiceAccess.Get<MeshProvider>();
}
