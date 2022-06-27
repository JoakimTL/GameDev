using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Engine.Data.Datatypes.Transforms;
using Engine.Rendering.Colors;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Standard.UI.Standard;
public abstract class RenderedUIElement<DATA, SHADER, V, SD> : UIElement where DATA : class, new() where SHADER : ShaderBundle where V : unmanaged where SD : unmanaged {

	private IUIConstraint<DATA>? _renderConstraint;
	protected readonly DATA _renderData;
	protected OpenSceneObject<V, SD> SceneObject { get; private set; }
	protected SceneInstanceData<SD> SceneInstanceData { get; private set; }

	public event Action<UIElement>? RenderingUpdated;

	public RenderedUIElement() {
		this.SceneObject = new OpenSceneObject<V, SD>();
		this.SceneObject.SetShaders<SHADER>();
		this.SceneObject.SetSceneData( this.SceneInstanceData = new SceneInstanceData<SD>( 1, 1 ) );
		SetSceneObject( this.SceneObject );
		TransformsUpdated += TransformUpdated;
		SetConstraint( new TestConstraint() );
		this._renderData = new DATA();
		Updated += RenderConstraintUpdate;
	}

	public void SetConstraint( IUIConstraint<DATA> constriant ) => this._renderConstraint = constriant;

	private void RenderConstraintUpdate( UIElement e, float time, float deltaTime ) {
		this._renderConstraint?.Apply( time, deltaTime, this._renderData );
		RenderingUpdated?.Invoke( this );
	}

	private void TransformUpdated( UIElement @this ) => this.SceneInstanceData.SetInstance( 0, GetSceneData() );

	protected abstract SD GetSceneData();
}

internal class TestRenderConstraint : IUIConstraint<ButtonData> {
	public int ExecutionOrder => 0;

	public void Apply( float time, float deltaTime, ButtonData data ) => data.Color = new Vector4( MathF.Sin( time  ) / 2 + .5f, MathF.Sin( time + MathF.PI * 2 / 3f ) / 2 + .5f, MathF.Sin( time  + MathF.PI * 4 / 3f ) / 2 + .5f, 1 );
}

internal class TestConstraint : IUIConstraint<Transform2> {
	public int ExecutionOrder => 0;

	public void Apply( float time, float deltaTime, Transform2 transform ) {
		transform.Scale = new Vector2( 0.5f, 1 );
		transform.Rotation = time;
	}
}

public class Button : RenderedUIElement<ButtonData, UIShaderBundle, Vertex2, Entity2SceneData> {

	public Button() {
		this.SceneObject.SetMesh( Resources.Render.Mesh2.Square );
		SetConstraint( new TestRenderConstraint() );
	}

	protected override Entity2SceneData GetSceneData() => new() { ModelMatrix = this.Transform.Matrix, Color = this._renderData.Color };
}

public class ButtonData {

	public Color16x4 Color { get; set; }

	public ButtonData() {
		this.Color = Color16x4.White;
	}
}

[Identification( "a6bc9616-05c6-4f5a-b568-caa1cd89fa82" )]
public class UIShaderBundle : ShaderBundle {
	public UIShaderBundle() : base( (0, Resources.Render.Shader.Pipelines.Get<UIShaderPipeline>()) ) { }

	public override bool UsesTransparency => true;
}

public class UIShaderPipeline : ShaderPipeline {
	public UIShaderPipeline() : base( typeof( UIShaderProgramVertex ), typeof( UIShaderProgramFragment ) ) { }
}

public class UIShaderProgramVertex : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "ui/uiShader.vert" ] );
}

public class UIShaderProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "ui/uiShader.frag" ] );
}