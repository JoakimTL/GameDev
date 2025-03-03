﻿using Engine.Logging;
using Engine.Modularity;
using Sandbox;

Log.LoggingLevel = Log.Level.NORMAL;

Startup.BeginInit()
	.WithModule<GameLogicModule>()
	.WithModule<SandboxRenderModule>()
	.Start();

//Lots of stuff: https://iquilezles.org/articles/

//TODO: Add camera (done?)
//TODO: Add input (done?)
//TODO: Add input to RenderEntity
//TODO: Add sound (https://github.com/naudio/NAudio ?)
//TODO: Partial icosphere (done?)
//TODO: Add text rendering (soon done?)
//TODO: Add GUI?
//TODO: Add render pipeline
//TODO: Render lines on separate FBO, fix 3d render pipeline (done)
//TODO: Add framebuffers (done?)
//TODO: Add grid for tiles near mouse pointer
//TODO: Add players and tile ownership
//TODO: Make tile ownership visible

//Game stuff:
//TODO: Add world entity and render the partial icosphere
//TODO: Add tiles
//TODO: Administrative centers. Not really, but every 4 tiles (one basetile above the last subdivision) is there everything "happens". The tiles are there for army movement and resource distribution. The tiles contain data while the "administrative centers" do the logic is another way to put it.
//TODO: Add players
//TODO: Add items
//TODO: Add characters and dynasties
//TODO: Add population and needs
//TODO: Add structures
//TODO: Add resources
//TODO: Add trade
//TODO: Add tech tree/research
//TODO: Add economy
//TODO: Add tech adoption per tile. A tile cannot gain the efficiencies of a tech unless it adopts the technology first. The rate of spread of adoption depends on the empire's policies, culture, inter-tile trade intensity, infrastructure (roads, rails, etc...), freedom of expression, freedom of movement, immigration of certain groups of people, emigration of certain groups of people (brain drain), and possibly other factors (ask chatgpt?).
//TODO: Add culture / religion
//TODO: Add politics
//TODO: Add spheres of influence

//TODO: Add armies
//TODO: Add combat

//TODO: Add diplomacy

//TODO: Add disease
//TODO: Add natural disasters
//TODO: Add skybox with stars and sun, make the planet rotate and "orbit" the sun. This is just to make the seasons felt