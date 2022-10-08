using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Rendering.Standard.UI.Standard.Text.Shaders;

namespace Engine.Rendering.Standard.UI.Standard.Text;
public class TextSceneObject : ClosedSceneObject<Vector2, TextGlyphRenderData> {

	private SceneInstanceData<TextGlyphRenderData> _sceneInstanceData;
	private readonly Texture _fontTexture;

	public TextSceneObject( Texture fontTexture, uint initialSize = 512 ) {
		SetMesh( Resources.Render.Mesh2.SquarePFX );
		SetShaders( Resources.Render.Shader.Bundles.Get<DistanceFieldGlyphShaderBundle>() ); //TODO: different shaders
		SetSceneData( this._sceneInstanceData = new SceneInstanceData<TextGlyphRenderData>( initialSize, 0 ) );
		this._fontTexture = fontTexture;
	}

	public void Resize( uint numGlyphs ) {
		if ( numGlyphs < this._sceneInstanceData.MaxInstances )
			return;
		this._sceneInstanceData.Dispose();
		SetSceneData( this._sceneInstanceData = new SceneInstanceData<TextGlyphRenderData>( numGlyphs, 0 ) );
	}

	private unsafe void SetData( TextGlyphRenderData* data, uint elementCount ) {
		if ( elementCount != this._sceneInstanceData.MaxInstances )
			Resize( elementCount );
		this._sceneInstanceData.SetInstances( 0, data, elementCount );
		this._sceneInstanceData.SetActiveInstances( elementCount );
	}

	public void UpdateSceneObjectData( Matrix4x4 transform, ulong textureHandle, IReadOnlyList<TextGlyphData> glyphs ) {
		GCHandle? pin = null;
		uint renderedGlyphs = (uint) glyphs.Count;
		try {
			unsafe {
				TextGlyphRenderData* dataPtr = null;
				if ( renderedGlyphs * sizeof( TextGlyphRenderData ) > 262144 ) { //0.25MiB 1/8 of the "normal" stack
					TextGlyphRenderData[] data = new TextGlyphRenderData[ renderedGlyphs ];
					pin = GCHandle.Alloc( data, GCHandleType.Pinned );
					dataPtr = (TextGlyphRenderData*) pin.Value.AddrOfPinnedObject().ToPointer();
				} else {
					byte* temporaryBytePointerForNoApparentReason = stackalloc byte[ (int) renderedGlyphs * sizeof( TextGlyphRenderData ) ];
					dataPtr = (TextGlyphRenderData*) temporaryBytePointerForNoApparentReason;
				}
				//Do render
				for ( int i = 0; i < renderedGlyphs; i++ )
					dataPtr[ i ] = new TextGlyphRenderData( transform, glyphs[ i ], textureHandle );
				SetData( dataPtr, renderedGlyphs );
			}
		} finally {
			if ( pin.HasValue ) {
				pin.Value.Free();
			}
		}
	}

	public override void Bind() => this._fontTexture.DirectBind();
}
