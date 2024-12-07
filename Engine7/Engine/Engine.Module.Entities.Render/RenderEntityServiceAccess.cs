using Engine.Module.Render.Input;

namespace Engine.Module.Entities.Render;

public sealed class RenderEntityServiceAccess( SceneInstanceProvider sceneInstanceProvider, CompositeVertexArrayProvider compositeVertexArrayProvider, ShaderBundleProvider shaderBundleProvider, MeshProvider meshProvider, UserInputEventService userInputEventService ) {
	private readonly SceneInstanceProvider _sceneInstanceProvider = sceneInstanceProvider;
	private readonly CompositeVertexArrayProvider _compositeVertexArrayProvider = compositeVertexArrayProvider;
	private readonly ShaderBundleProvider _shaderBundleProvider = shaderBundleProvider;
	private readonly MeshProvider _meshProvider = meshProvider;
	private readonly UserInputEventService _userInputEventService = userInputEventService;

	internal SceneInstanceProvider SceneInstanceProvider => this._sceneInstanceProvider;
	internal CompositeVertexArrayProvider CompositeVertexArrayProvider => this._compositeVertexArrayProvider;
	internal ShaderBundleProvider ShaderBundleProvider => this._shaderBundleProvider;
	internal MeshProvider MeshProvider => this._meshProvider;
	internal UserInputEventService UserInputEventService => this._userInputEventService;
}