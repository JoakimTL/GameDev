﻿using Engine.Module.Render.Ogl.OOP.Textures;
using Engine.Standard.Render.Meshing.Services;
using Engine.Standard.Render.Shaders;

namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class TexturedBackground : UserInterfaceComponentBase {
	private readonly SceneInstance<Entity2TexturedSceneData> _sceneInstance;

	private Vector4<double> _color = (1, 1, 1, 1);
	public Vector4<double> Color {
		get => _color;
		set {
			if (_color == value)
				return;
			_color = value;
			_changed = true;
		}
	}

	private OglTexture _texture;
	public OglTexture Texture {
		get => _texture;
		set {
			if (_texture == value)
				return;
			_texture = value;
			_reference = null;
			_changed = true;
		}
	}

	private TextureReference? _reference;

	private bool _changed;

	public TexturedBackground( UserInterfaceElementBase element, OglTexture texture ) : base( element ) {
		_sceneInstance = CreateSceneInstance();
		_texture = texture;
	}

	public TexturedBackground( UserInterfaceComponentBase parent, OglTexture texture ) : base( parent, 0 ) {
		_sceneInstance = CreateSceneInstance();
		_texture = texture;
	}

	private SceneInstance<Entity2TexturedSceneData> CreateSceneInstance() {
		SceneInstance<Entity2TexturedSceneData> sceneInstance = Element.UserInterfaceServiceAccess.RequestSceneInstance<SceneInstance<Entity2TexturedSceneData>>( RenderLayer );
		sceneInstance.SetVertexArrayObject( Element.UserInterfaceServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex2, Entity2TexturedSceneData>() );
		sceneInstance.SetShaderBundle( Element.UserInterfaceServiceAccess.ShaderBundleProvider.GetShaderBundle<TexturedShade2ShaderBundle>() );
		sceneInstance.SetMesh( Element.UserInterfaceServiceAccess.Get<PrimitiveMesh2Provider>().Get( Meshing.Primitive2.Rectangle ) );
		return sceneInstance;
	}


	protected override void OnPlacementChanged() => UpdateInstance();

	protected override void OnUpdate( double time, double deltaTime ) {
		if (!_changed)
			return;
		_changed = false;
		UpdateInstance();
	}

	private void UpdateInstance() {
		_reference ??= _texture.GetTextureReference();
		Vector4<ushort> color = (Color * ushort.MaxValue).Clamp<Vector4<double>, double>( 0, ushort.MaxValue ).CastSaturating<double, ushort>();
		_sceneInstance.Write( new Entity2TexturedSceneData( TransformInterface.Matrix.CastSaturating<double, float>(), color, _reference.GetHandle() ) );
	}

	protected override bool InternalDispose() => true;

	protected internal override void DoHide() {
		_sceneInstance.SetActive( false );
	}

	protected internal override void DoShow() {
		_sceneInstance.SetActive( true );
	}
}
