using Engine.Rendering.Utilities;

namespace Engine.Modularity.Modules.Submodules;
public class GlfwSubmodule : Submodule {

	private bool _mainModule;

	public GlfwSubmodule() : base( false ) {
		OnInitialization += Initialized;
		OnUpdate += Updated;
	}

	protected void Initialized() {
		if ( OpenGLUtilities.Initialized ) {
			Remove();
			this._mainModule = false;
			return;
		}
		OpenGLUtilities.InitializeGL();
		this._mainModule = true;
	}

	protected static void Updated( float time, float deltaTime ) => OpenGLUtilities.PollEvents();

	protected override bool OnDispose() {
		if ( this._mainModule )
			OpenGLUtilities.Terminate();
		return true;
	}
}
