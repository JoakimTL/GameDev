using Engine.LinearAlgebra;
using System.Collections.Generic;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public class FontMetadata {
		private Dictionary<char, Symbol> charData;

		public int PaddingLeft { get; private set; }
		public int PaddingRight { get; private set; }
		public int PaddingUp { get; private set; }
		public int PaddingDown { get; private set; }
		public int SpacingX { get; private set; }
		public int SpacingY { get; private set; }
		public int TotalLineHeight { get; private set; }
		public int BaseHeight { get; private set; }
		public int ScaleX { get; private set; }
		public int ScaleY { get; private set; }
		public float LineHeight { get; private set; }
		public Symbol Space { get; private set; }
		public Symbol LineBreak { get; private set; }

		public FontMetadata( string filePath ) {
			charData = new Dictionary<char, Symbol>();
			string[] data = System.IO.File.ReadAllLines( filePath );

			ReadData( data );

			Space = this[ ' ' ];
			LineBreak = new Symbol( '\n', 0, 0, 0, 0, 0, 0 );
		}

		private void ReadData( string[] data ) {
			{   //Line 0
				string[] line = data[ 0 ].Split( ' ' );
				if( line[ 0 ].StartsWith( "info" ) ) {
					int[] padding = ExtractLine( line, "padding" );
					int[] spacing = ExtractLine( line, "spacing" );
					PaddingUp = padding[ 0 ];
					PaddingLeft = padding[ 1 ];
					PaddingDown = padding[ 2 ];
					PaddingRight = padding[ 3 ];
					SpacingX = spacing[ 0 ];
					SpacingY = spacing[ 1 ];
				}
			}

			{   //Line 1
				string[] line = data[ 1 ].Split( ' ' );
				if( line[ 0 ].StartsWith( "common" ) ) {
					TotalLineHeight = ExtractLine( line, "lineHeight" )[ 0 ];
					BaseHeight = ExtractLine( line, "base" )[ 0 ];
					ScaleX = ExtractLine( line, "scaleW" )[ 0 ];
					ScaleY = ExtractLine( line, "scaleH" )[ 0 ];
				}
			}

			LineHeight = (float) ( BaseHeight + SpacingY + PaddingUp + PaddingDown ) / TotalLineHeight;

			//Read Characters
			for( int i = 2; i < data.Length; i++ ) {
				if( data[ i ].StartsWith( "char " ) ) {
					string[] line = data[ i ].Split( ' ' );

					int id = ExtractLine( line, "id" )[ 0 ];

					float x = ( (float) ExtractLine( line, "x" )[ 0 ] ) / ScaleX;
					float y = ( (float) ExtractLine( line, "y" )[ 0 ] ) / ScaleY;
					float tW = ( (float) ExtractLine( line, "width" )[ 0 ] ) / ScaleX;
					float tH = ( (float) ExtractLine( line, "height" )[ 0 ] ) / ScaleY;

					float w = ExtractLine( line, "width" )[ 0 ];
					float h = ExtractLine( line, "height" )[ 0 ];
					float xOff = ExtractLine( line, "xoffset" )[ 0 ] + PaddingRight + PaddingLeft;
					float yOff = ExtractLine( line, "yoffset" )[ 0 ];
					float xAdv = ExtractLine( line, "xadvance" )[ 0 ];

					charData[ (char) id ] = new Symbol(
						(char) id,
						new Vector4( x, y, tW, tH ),
						w / TotalLineHeight,
						h / TotalLineHeight,
						xOff / TotalLineHeight,
						yOff / TotalLineHeight,
						xAdv / TotalLineHeight
					);
				}
			}
		}

		public Symbol this[ char c ] {
			get {
				if( charData.TryGetValue( c, out Symbol ch ) )
					return ch;
				return Space;
			}
		}

		private static int[] ExtractLine( string[] line, string name ) {
			for( int i = 0; i < line.Length; i++ )
				if( line[ i ].StartsWith( name ) ) {
					string[] data = line[ i ].Split( '=' )[ 1 ].Split( ',' );

					int[] @return = new int[ data.Length ];
					for( int j = 0; j < @return.Length; j++ )
						@return[ j ] = int.Parse( data[ j ] );
					return @return;
				}
			return new int[ 0 ];
		}


	}
}
