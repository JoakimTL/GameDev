using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.LinearAlgebra;
using Engine.Utilities.Data;
using System;
using System.Collections.Generic;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public struct DisplayLetter {
		
		public Vector2 Translation;
		public Vector2 Size;
		public Vector2 TextureOffset;
		public Vector2 TextureSize;
		public Vector4b Color;

		public DisplayLetter( Vector2 translation, Vector2 size, Vector2 texOff, Vector2 texSize, Vector4b color ) {
			Translation = translation;
			Size = size;
			TextureOffset = texOff;
			TextureSize = texSize;
			Color = color;
		}

		public void InjectData( List<byte> list ) {
			list.AddRange( DataTransform.GetBytes( Translation ) );
			list.AddRange( DataTransform.GetBytes( Size ) );
			list.AddRange( DataTransform.GetBytes( TextureOffset ) );
			list.AddRange( DataTransform.GetBytes( TextureSize ) );
			list.Add( Color.X );
			list.Add( Color.Y );
			list.Add( Color.Z );
			list.Add( Color.W );
		}
	}
}
