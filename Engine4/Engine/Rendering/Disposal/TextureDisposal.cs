using OpenGL;

namespace Engine.Rendering.Disposal;
public class TextureDisposal : DisposableIdentifiable {

	public readonly ulong Handle;
	public readonly uint TextureID;
	public bool Resident { get; internal set; }

	public TextureDisposal( string name, ulong handle, uint textureId, bool isResident ) : base( name ) {
		this.Handle = handle;
		this.TextureID = textureId;
		this.Resident = isResident;
	}

	protected override bool OnDispose() {
		if ( this.Resident ) {
			Gl.MakeTextureHandleNonResidentARB( this.Handle );
			this.LogLine( $"Made non-resident!", Log.Level.LOW );
		}

		Gl.DeleteTextures( new uint[] { this.TextureID } );
		this.LogLine( "Disposed on context thread!", Log.Level.HIGH, ConsoleColor.Cyan );
		return true;
	}
}
