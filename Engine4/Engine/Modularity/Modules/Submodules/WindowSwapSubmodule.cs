using Engine.Structure;

namespace Engine.Modularity.Modules.Submodules;

[ProcessAfter( typeof( WindowCreationSubmodule ), typeof( IUpdateable ) )]
[ProcessAfter( typeof( RenderSubmodule ), typeof( IUpdateable ) )]
[ProcessAfter( typeof( GlfwSubmodule ), typeof( IUpdateable ) )]
public class WindowSwapSubmodule : Submodule {

	public WindowSwapSubmodule() : base( false ) {
		OnUpdate += Updated;
	}

	private void Updated( float time, float deltaTime ) => GLFW.Glfw.SwapBuffers( Resources.Render.Window.Pointer );

	protected override bool OnDispose() => true;

}

