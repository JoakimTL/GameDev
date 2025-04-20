using Engine.Modularity;

Startup.BeginInit()
	.WithModule<Civs.Logic.CivsGameLogicModule>()
	.WithModule<Civs.Render.CivsRenderModule>()
	.Start();
