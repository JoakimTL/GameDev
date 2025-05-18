using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.Providers;

namespace Engine.Module.Render.Entities;

public sealed class RenderEntityServiceAccess( RenderServiceAccess renderServiceAccess, CapturableUserInputEventService capturableUserInputEventService ) : DisposableIdentifiable {
	private readonly RenderServiceAccess _renderServiceAccess = renderServiceAccess;
	private readonly SceneInstanceProvider _sceneInstanceProvider = renderServiceAccess.Get<SceneInstanceProvider>();
	private readonly CompositeVertexArrayProvider _compositeVertexArrayProvider = renderServiceAccess.Get<CompositeVertexArrayProvider>();
	private readonly ShaderBundleProvider _shaderBundleProvider = renderServiceAccess.Get<ShaderBundleProvider>();
	private readonly MeshProvider _meshProvider = renderServiceAccess.Get<MeshProvider>();
	private readonly RenderEntityInput _input = new( capturableUserInputEventService );
	private readonly CameraProvider _cameraProvider = renderServiceAccess.Get<CameraProvider>();

	internal SceneInstanceProvider SceneInstanceProvider => this._sceneInstanceProvider;
	public CompositeVertexArrayProvider CompositeVertexArrayProvider => this._compositeVertexArrayProvider;
	public ShaderBundleProvider ShaderBundleProvider => this._shaderBundleProvider;
	public MeshProvider MeshProvider => this._meshProvider;
	public CameraProvider CameraProvider => this._cameraProvider;
	public RenderEntityInput Input => this._input;

	public T Get<T>() where T : IServiceProvider => _renderServiceAccess.Get<T>();

	protected override bool InternalDispose() {
		_input.Dispose();
		return true;
	}
}