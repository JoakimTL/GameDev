using Engine.Modularity;

Startup.BeginInit()
	.WithModule<Civlike.Logic.CivsGameLogicModule>()
	.WithModule<Civlike.Client.Render.CivsRenderModule>()
	.Start();
