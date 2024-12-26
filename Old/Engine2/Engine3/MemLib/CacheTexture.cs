using Engine.Graphics.Objects;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Engine.MemLib {
	public class CacheTexture : Cache<string, Texture> {

		public Texture BlankWhite { get; private set; }
		public Texture BlankBlack { get; private set; }

		private readonly string baseDirectory, extensionName;

		public CacheTexture( VaultReferences refVault, string baseDirectory, string extensionName ) : base( refVault, true ) {
			this.baseDirectory = baseDirectory;
			this.extensionName = extensionName;
		}

		public void Initialize() {
			Bitmap BlankWhiteBMP = new Bitmap( 1, 1 );
			BlankWhiteBMP.SetPixel( 0, 0, Color.White );
			BlankWhite = new Texture( "BlankWhite", BlankWhiteBMP );
			Bitmap BlankBlackBMP = new Bitmap( 1, 1 );
			BlankBlackBMP.SetPixel( 0, 0, Color.Black );
			BlankBlack = new Texture( "BlankBlack", BlankBlackBMP );

			Add( BlankWhite.Name, BlankWhite );
			Add( BlankBlack.Name, BlankBlack );
		}

		public Texture Get( string name, TextureMagFilter filter ) {
			if( cacheReferenceDictionary.TryGetValue( name, out uint refId ) )
				return ReferenceVault.TryGet<Texture>( refId );
			if( HandlesNewAccessKeys )
				return HandleNewObjectCustom( name, filter );
			return UnhandledNewObject();
		}

		protected override Texture HandleNewObject( string key ) {
			bool v = Texture.FromFile( $"{baseDirectory}\\{key}.{extensionName}", out Texture t );
			if( v )
				Add( key, t );
			return t;
		}

		private  Texture HandleNewObjectCustom( string key, TextureMagFilter filter ) {
			bool v = Texture.FromFile( $"{baseDirectory}\\{key}.{extensionName}", out Texture t, filter );
			if( v )
				Add( key, t );
			return t;
		}

	}
}
