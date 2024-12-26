using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Junctions;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.Scenes;
using Engine.Pipelines;
using Engine.Utilities.Data.Boxing;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Pipelines {
	public class RenderPipeline2 : Pipeline {
		public RenderPipeline2( GLWindow window, MutableSinglet<View2> uiView, MutableSinglet<View2> entityView, SceneMeshMaterial<SceneObjectData2> sceneUI, SceneMeshMaterial<SceneObjectData2> sceneEntity ) : base( "Default Rendering Pipeline 2d" ) {
			InsertLast( new Junction( "Enable 2D Rendering", delegate () {
				Gl.Disable( EnableCap.CullFace );
				Gl.Disable( EnableCap.DepthTest );
				Gl.Enable( EnableCap.Blend );
				Gl.BlendEquation( BlendEquationMode.FuncAdd );
				Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
			} ) );

			InsertLast( new JunctionBindWindow( "Unbind Buffer", window ) );

			InsertLast( new JunctionRenderScene<SceneObjectData2, View2>( "Render Entities", entityView, sceneEntity ) );

			InsertLast( new JunctionRenderScene<SceneObjectData2, View2>( "Render UI", uiView, sceneUI ) );
		}
	}
}
