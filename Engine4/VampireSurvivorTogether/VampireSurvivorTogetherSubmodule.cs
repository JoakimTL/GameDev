//using Engine;
//using Engine.Modularity.Domains;
//using Engine.Modularity.ECS;
//using Engine.Modularity.ECS.Components;
//using Engine.Modularity.ECS.Networking;
//using Engine.Modularity.Modules.Submodules;
//using Engine.Networking;
//using Engine.Rendering.Pipelines;
//using Engine.Rendering.Standard.Scenes;
//using Engine.Structure;

//namespace VampireSurvivorTogether;

//[ProcessAfter( typeof( ContextUpdateSubmodule ), typeof( IUpdateable ) )]
//public class VampireSurvivorTogetherSubmodule : Module {

//	private Scene scene2;
//	private SceneRenderer sceneRenderer;
//	private Engine.Modularity.ECS.Organization.EntityScene2Manager ec2m;


//	public VampireSurvivorTogetherSubmodule() : base( true ) {
//		OnInitialization += Initialized;
//		OnUpdate += Updated;
//	}

//	private void Initialized() {
//		Resources.Render.Window.WindowEvents.Closing += Remove;
//		Resources.Render.PipelineManager.AddPipeline<Render2Pipeline>();
//		Resources.Render.PipelineManager.AddPipeline<RenderUIPipeline>();

//		this.scene2 = new LayeredScene();
//		this.sceneRenderer = new SceneRenderer( this.scene2 );
//		Resources.Render.PipelineManager.GetOrAdd<Render2Pipeline>().Scenes.Add( this.sceneRenderer );
//		this.ec2m = new Engine.Modularity.ECS.Organization.EntityScene2Manager( Service<EntityManager>(), this.scene2 );
//		Service<EntityNetworkManager>();
//		Service<ClientEntityControllerManager>();
//	}

//	private void Updated( float time, float deltaTime ) {
//		this.ec2m.Update();
//		if ( Resources.Render.Window.KeyboardEvents[ GLFW.Keys.G ] )
//			Console.WriteLine( "break!" );
//		var playerEntities = Singleton<EntityManager>().GetPlayerEntitiesOrDefault(Singleton<NetworkManager>().ServerProvidedId);
//		if ( playerEntities is not null ) {
//			Entity? player = playerEntities.FirstOrDefault().Value;
//			if ( player is null )
//				return;
//			if ( player.TryGetComponent( out Transform2Component? tra ) ) {
//				var v = Resources.Render.PipelineManager.GetOrAdd<Render2Pipeline>().View;
//				v.Translation += ( tra.Transform.Translation - v.Translation ) * deltaTime;
//			}

//		}
//	}

//	protected override bool OnDispose() {
//		return true;
//	}
//}
