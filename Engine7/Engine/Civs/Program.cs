using Engine.Modularity;

Startup.BeginInit()
	//.WithModule<CivsGameLogicModule>()
	.WithModule<Civs.Render.CivsRenderModule>()
	.Start();
