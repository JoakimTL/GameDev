

//using Engine;
//using Engine.Modularity.Domains;
//using VampireSurvivorTogether;

Console.Write( "start server (Y/N)? " );
//string? serverPromt = Console.ReadLine();
//bool startServer = false;
//if ( serverPromt?.ToLower().Equals( "y" ) ?? false )
//	startServer = true;

//var modules = new List<Module>();
//modules.Add( ModuleGenerator.CreateModule(
//		"Vampire Survivor Together", 0, true,
//		ModuleGenerator.GetRenderSubmodules(),
//		ModuleGenerator.GetClientSubmodules(),
//		new[] { typeof( RenderSubmodule ), typeof( VampireSurvivorTogetherSubmodule ) }
//	) );

//if ( startServer )
//	modules.Add( ModuleGenerator.CreateModule( "Server", 1, false, 5,
//	ModuleGenerator.GetServerSubmodules(),
//	new[] { typeof( VampireSurvivorTogetherServerSubmodule ) } ) );

//Startup.Start(modules.ToArray());

////TODO:

///*
// Create client command component rather than input, as input should be remappable, while they still represent the same commands (example: W -> Move Up)
// Allow for determinism, if a component has it's data changed in a deterministic way (say a velocity changes translation, shouldn't need to update translation each tick then?)
// Allow for commands to be sent from clients to server, and from server to client. A great usecase for this would be entity creation.
// */