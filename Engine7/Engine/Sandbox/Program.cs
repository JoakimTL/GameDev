using Engine.Modularity;
using Sandbox;

Startup.BeginInit()
	.WithModule<TestModule>()
	.WithModule<SandboxRenderModule>()
	.Start();

/*
 * 
 */