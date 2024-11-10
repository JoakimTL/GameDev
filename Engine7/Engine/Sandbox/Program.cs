using Engine;
using Engine.Modularity;
using Sandbox;

Startup.BeginInit()
	.WithModule<TestModule>()
	.Start();
