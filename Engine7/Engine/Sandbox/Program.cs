using Engine.Logging;
using Engine.Modularity;
using Sandbox;

Log.LoggingLevel = Log.Level.VERBOSE;

Startup.BeginInit()
	.WithModule<GameLogicModule>()
	.WithModule<SandboxRenderModule>()
	.Start();

//Lots of stuff: https://iquilezles.org/articles/

//TODO: Add camera (done?)
//TODO: Add render pipeline
//TODO: Add input (done?)
//TODO: Add input to RenderEntity
//TODO: Add sound (https://github.com/naudio/NAudio ?)
//TODO: Partial icosphere
//TODO: Add text rendering (soon done?)
//TODO: Add GUI?

//Game stuff:
//TODO: Add world entity and render the partial icosphere
//TODO: Add tiles
//TODO: Add players
//TODO: Add items
//TODO: Add population and needs
//TODO: Add structures
//TODO: Add resources
//TODO: Add trade
//TODO: Add tech tree/research
//TODO: Add culture / religion
//TODO: Add politics
//TODO: Add spheres of influence

//TODO: Add armies
//TODO: Add combat

//TODO: Add diplomacy