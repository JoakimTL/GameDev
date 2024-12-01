using Engine.Module.Render.Input;

namespace Engine.Module.Entities.Render;

public sealed class RenderEntityServiceAccess( SceneInstanceProvider sceneInstanceProvider, CompositeVertexArrayProvider compositeVertexArrayProvider, ShaderBundleProvider shaderBundleProvider, MeshProvider meshProvider, UserInputEventService userInputEventService ) {
	private readonly SceneInstanceProvider _sceneInstanceProvider = sceneInstanceProvider;
	private readonly CompositeVertexArrayProvider _compositeVertexArrayProvider = compositeVertexArrayProvider;
	private readonly ShaderBundleProvider _shaderBundleProvider = shaderBundleProvider;
	private readonly MeshProvider _meshProvider = meshProvider;
	private readonly UserInputEventService _userInputEventService = userInputEventService;

	internal SceneInstanceProvider SceneInstanceProvider => _sceneInstanceProvider;
	internal CompositeVertexArrayProvider CompositeVertexArrayProvider => _compositeVertexArrayProvider;
	internal ShaderBundleProvider ShaderBundleProvider => _shaderBundleProvider;
	internal MeshProvider MeshProvider => _meshProvider;
	internal UserInputEventService UserInputEventService => _userInputEventService;
}