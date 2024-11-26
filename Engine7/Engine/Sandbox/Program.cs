using Engine.Logging;
using Engine.Modularity;
using Sandbox;

Log.LoggingLevel = Log.Level.VERBOSE;

Startup.BeginInit()
	.WithModule<GameLogicModule>()
	.WithModule<SandboxRenderModule>()
	.Start();

/*
 * 
 */