using Engine.Module.Render.Entities.Providers;
using Engine.Module.Render.Input;

namespace Engine.Module.Entities.Render;

public sealed class RenderEntityServiceAccess( IInstanceProvider instanceProvider, SceneInstanceProvider sceneInstanceProvider, CompositeVertexArrayProvider compositeVertexArrayProvider, ShaderBundleProvider shaderBundleProvider, MeshProvider meshProvider, UserInputEventService userInputEventService, CameraProvider cameraProvider ) : IInitializable {
	private readonly IInstanceProvider _instanceProvider = instanceProvider;
	private readonly SceneInstanceProvider _sceneInstanceProvider = sceneInstanceProvider;
	private readonly CompositeVertexArrayProvider _compositeVertexArrayProvider = compositeVertexArrayProvider;
	private readonly ShaderBundleProvider _shaderBundleProvider = shaderBundleProvider;
	private readonly MeshProvider _meshProvider = meshProvider;
	private readonly UserInputEventService _userInputEventService = userInputEventService;
	private readonly CameraProvider _cameraProvider = cameraProvider;

	internal SceneInstanceProvider SceneInstanceProvider => this._sceneInstanceProvider;
	public CompositeVertexArrayProvider CompositeVertexArrayProvider => this._compositeVertexArrayProvider;
	public ShaderBundleProvider ShaderBundleProvider => this._shaderBundleProvider;
	public MeshProvider MeshProvider => this._meshProvider;
	public CameraProvider CameraProvider => this._cameraProvider;
	//TODO Make provider or handle differently!
	public UserInputEventService UserInputEventService => this._userInputEventService;
	public T Get<T>() where T : IRenderEntityServiceProvider => _instanceProvider.Get<T>();

	public void Initialize() {
		foreach (Type t in TypeManager.Registry.DerivedTypes.Where( p => p.IsAssignableTo( typeof( IRenderEntityServiceProvider ) ) ))
			_instanceProvider.Catalog.Host( t );
	}
}
