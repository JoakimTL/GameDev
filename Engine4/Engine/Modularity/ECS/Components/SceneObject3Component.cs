namespace Engine.Modularity.ECS.Components;

/*public class SceneObject3Component : SceneObjectComponentBase<Vertex3, Entity3SceneData> {

	private Transform3Component? _transform;
	private Render3Component? _render;

	public SceneObject3Component() {
		this._sceneData = new Entity3SceneData() {
			NormalMapped = 0,
			DiffuseTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			NormalTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			GlowTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			LightingTextureHandle = Resources.Render.Textures.White1x1.GetHandleDirect(),
			Color = Rendering.Colors.Color16x4.White
		};
	}

	protected override void ParentSet() {
		this._transform = this.Parent.GetComponent<Transform3Component>();
		this._render = this.Parent.GetComponent<Render3Component>();
		this.Parent.ComponentAdded += ComponentAdded;
		this.Parent.ComponentRemoved += ComponentRemoved;
		if ( this._transform is not null ) {
			this._transform.Changed += ComponentsChanged;
			ComponentsChanged( this._transform );
		}
		if ( this._render is not null ) {
			this._render.Changed += ComponentsChanged;
			ComponentsChanged( this._render );
		}
	}

	private void ComponentsChanged( Component? obj ) => TriggerChanged();
	/*
	private void RenderChanged( Component? obj ) {
		if ( this._render is not null ) {
			VertexMesh<Vertex3>? mesh = Resources.Render.BufferedMesh.Get<Vertex3>( this._render.Mesh );
			ShaderBundle? shaderBundle = Resources.Render.Shader.Bundles.Get( this._render.ShaderBundle );
			if ( mesh is not null && shaderBundle is not null ) {
				this._sceneObject.SetMesh( mesh );
				this._sceneObject.SetShaders( shaderBundle );
			}
			this._sceneData.Color = this._render.Color;
		} else {
			this._sceneObject.SetShaders( Resources.Render.Shader.Bundles.Get<Entity3ShaderBundle>() );
			this._sceneObject.SetMesh( Resources.Render.Mesh3.Cube );
		}
		this._sceneObject.SceneData?.SetInstance( 0, this._sceneData );
	}

	private void TransformChanged( Component? obj ) {
		this._sceneData.ModelMatrix = this._transform?.Transform.Matrix ?? System.Numerics.Matrix4x4.Identity;
		this._sceneObject.SceneData?.SetInstance( 0, this._sceneData );
	}
	*/
/*
	private void ComponentAdded( Entity e, Component c ) {
		if ( c is Transform3Component t3 ) {
			this._transform = t3;
			this._transform.Changed += ComponentsChanged;
			ComponentsChanged( this._transform );
		} else if ( c is Render3Component r3 ) {
			this._render = r3;
			this._render.Changed += ComponentsChanged;
			ComponentsChanged( this._render );
		}
	}

	private void ComponentRemoved( Entity e, Component c ) {
		if ( c is Transform3Component ) {
			if ( this._transform is not null )
				this._transform.Changed -= ComponentsChanged;
			this._transform = null;
			ComponentsChanged( this._transform );
		} else if ( c is Render3Component ) {
			if ( this._render is not null )
				this._render.Changed -= ComponentsChanged;
			this._render = null;
			ComponentsChanged( this._render );
		}
	}
}*/