using Engine.Structure;

namespace Engine.Modularity.Modules.Submodules;

[ProcessAfter( typeof( WindowCreationSubmodule ), typeof( IUpdateable ) )]
[ProcessBefore( typeof( GlfwSubmodule ), typeof( IDisposable ) )]
public class ContextUpdateSubmodule : Submodule {
	public ContextUpdateSubmodule() : base( false ) {
		OnUpdate += Updated;
	}

	private void Updated( float time, float deltaTime ) => Resources.Update( time, deltaTime );

	protected override bool OnDispose() {
		Resources.Dispose();
		return true;
	}
}
