﻿using Engine.Module.Render.Ogl.Services;
using Engine.Transforms.Camera;
using Engine.Transforms;

namespace Engine.Module.Render;

public sealed class CameraSuite : Identifiable {

	public CameraSuite( string name, WindowService windowService ) {
		View2 = new();
		View3 = new();
		Projection2 = new( windowService.Window, 1, -1, 1 );
		Projection3 = new( windowService.Window, 90 );
		Camera2 = new Camera( View2, Projection2 );
		Camera3 = new Camera( View3, Projection3 );
		Nickname = Name = name;
	}

	public string Name { get; }

	public View2 View2 { get; }
	public View3 View3 { get; }

	public Orthographic.Dynamic Projection2 { get; }
	public Perspective.Dynamic Projection3 { get; }

	public IMatrixProvider<float> Camera2 { get; }
	public IMatrixProvider<float> Camera3 { get; }
}