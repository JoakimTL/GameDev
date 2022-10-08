namespace Engine.Modularity.ECS.Components;

/*
public class SceneObject2Component : SceneObjectComponentBase<Vertex2, Entity2SceneData> {

	private Entity2SceneData _sceneData;
	private Transform2Component? _transform;
	private Render2Component? _render;

	public SceneObject2Component() {
		this._sceneData = new Entity2SceneData() {
			Color = Rendering.Colors.Color16x4.White
		};
	}

	protected override void ParentSet() {
		this._transform = this.Parent.GetComponent<Transform2Component>();
		this._render = this.Parent.GetComponent<Render2Component>();
		this.Parent.ComponentAdded += ComponentAdded;
		this.Parent.ComponentRemoved += ComponentRemoved;
		if ( this._transform is not null ) {
			this._transform.Changed += TransformChanged;
			TransformChanged( this._transform );
		}
		if ( this._render is not null ) {
			this._render.Changed += RenderChanged;
			RenderChanged( this._render );
		}
	}

	private void RenderChanged( Component? obj ) {
		if ( this._render is not null ) {
			VertexMesh<Vertex2>? mesh = Resources.Render.BufferedMesh.Get<Vertex2>( this._render.Mesh );
			ShaderBundle? shaderBundle = Resources.Render.Shader.Bundles.Get( this._render.ShaderBundle );
			if ( mesh is not null && shaderBundle is not null ) {
				this._sceneObject.SetMesh( mesh );
				this._sceneObject.SetShaders( shaderBundle );
			}
			this._sceneData.Color = this._render.Color;
		} else {
			this._sceneObject.SetShaders( Resources.Render.Shader.Bundles.Get<Entity2ShaderBundle>() );
			this._sceneObject.SetMesh( Resources.Render.Mesh3.Cube );
		}
		this._sceneObject.SceneData?.SetInstance( 0, this._sceneData );
	}

	private void TransformChanged( Component? obj ) {
		this._sceneData.ModelMatrix = this._transform?.Transform.Matrix ?? System.Numerics.Matrix4x4.Identity;
		this._sceneObject.SceneData?.SetInstance( 0, this._sceneData );
	}

	private void ComponentAdded( Entity e, Component c ) {
		if ( c is Transform2Component t3 ) {
			this._transform = t3;
			this._transform.Changed += TransformChanged;
			TransformChanged( this._transform );
		} else if ( c is Render2Component r3 ) {
			this._render = r3;
			this._render.Changed += TransformChanged;
			RenderChanged( this._render );
		}
	}

	private void ComponentRemoved( Entity e, Component c ) {
		if ( c is Transform2Component ) {
			if ( this._transform is not null )
				this._transform.Changed -= TransformChanged;
			this._transform = null;
			TransformChanged( this._transform );
		} else if ( c is Render2Component ) {
			if ( this._render is not null )
				this._render.Changed -= TransformChanged;
			this._render = null;
			RenderChanged( this._render );
		}
	}
}
*/
