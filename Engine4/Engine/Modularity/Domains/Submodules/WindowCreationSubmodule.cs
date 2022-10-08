//using Engine.Rendering.Utilities;
//using Engine.Structure;

//namespace Engine.Modularity.Modules.Submodules;

//[ProcessAfter( typeof( GlfwSubmodule ), typeof( IUpdateable ) )]
//public class WindowCreationSubmodule : Submodule {

//	public WindowCreationSubmodule() : base( false ) {
//		OnInitialization += Initialized;
//	}

//	private void Initialized() {
//		OpenGLUtilities.WaitInitialization();
//		Resources.Render.InitializeGl();
//		Remove();
//	}

//	protected override bool OnDispose() => true;
//}
