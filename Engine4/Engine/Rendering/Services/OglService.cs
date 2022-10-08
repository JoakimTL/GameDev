using Engine.Rendering.Utilities;

namespace Engine.Rendering.Services;
public sealed class OglService : ModuleService, IUpdateable, IInitializable {
	public bool Active => true;

	public bool Intitialized { get; private set; } = false;

	//Initialize Glfw, then Ogl. If Glfw is initialized, this step is ignored, except this module is placed on an internal list in the oglutils static class. If all initialization services terminate the 
	public void Initialize() {
		if ( !OpenGLUtilities.InitializeGL() )
			Dispose();
		this.Intitialized = true;
	}

	public void Update( float time, float deltaTime ) => OpenGLUtilities.PollEvents();

	protected override bool OnDispose() {
		if ( this.Intitialized )
			OpenGLUtilities.Terminate();
		return true;
	}
}

