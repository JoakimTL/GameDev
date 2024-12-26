using Engine.Utilities.Data;
using Engine.Utilities.Data.Boxing;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Graphics.Objects {
	public class Material : ICountable {

		private static readonly List<IReadOnlyList<uint>> sequences = new List<IReadOnlyList<uint>>() { new List<uint>() };

		public string Name { get; private set; }
		private uint id;
		public uint ID { get => GetID(); }
		private bool needsAssignment;
		protected ShaderBind shaderBind;
		private readonly List<ImmutableDuo<TextureUnit, Texture>> textures;
		private readonly List<uint> textureSequence;

		public Material( string name ) {
			this.Name = $"Material[{name}:{ID}]";
			this.textures = new List<ImmutableDuo<TextureUnit, Texture>>();
			textureSequence = new List<uint>();
			id = 0;
			shaderBind = default;
		}

		protected void AddTextures( ImmutableDuo<TextureUnit, Texture> textureCombo ) {
			textures.Add( textureCombo );
			textureSequence.Add( (uint) textureCombo.ValueA );
			textureSequence.Add( textureCombo.ValueB.TextureID );
			needsAssignment = true;
		}

		public void Bind() {
			for( int i = 0; i < textures.Count; i++ ) {
				Gl.ActiveTexture( textures[ i ].ValueA );
				Gl.BindTexture( TextureTarget.Texture2d, textures[ i ].ValueB.TextureID );
			}
		}

		public void BindShader( Shader s ) => shaderBind?.Invoke( s );

		public void Unbind() {
			for( int i = 0; i < textures.Count; i++ ) {
				Gl.ActiveTexture( textures[ i ].ValueA );
				Gl.BindTexture( TextureTarget.Texture2d, 0 );
			}
		}

		private uint GetID() {
			if( needsAssignment )
				id = AssignID();
			return id;
		}

		private uint AssignID() {
			lock( sequences ) {
				needsAssignment = false;
				if( textureSequence.Count == 0 )
					return 0;

				for( int i = 0; i < sequences.Count; i++ ) {
					if( sequences[ i ].SequenceEqual( textureSequence ) )
						return (uint) i;
				}

				sequences.Add( textureSequence );
				return (uint) sequences.Count - 1;
			}
		}

		public override int GetHashCode() {
			return (int) ID;
		}

		public override bool Equals( object obj ) {
			if( !( obj is Material o ) )
				return false;
			return Equals( o );
		}

		public bool Equals( Material obj ) {
			return obj.ID == ID;
		}

		public static bool operator ==( Material a, Material b ) {
			if( a is null || b is null )
				return false;
			return a.Equals( b );
		}

		public static bool operator !=( Material a, Material b ) {
			return !( a == b );
		}

		public override string ToString() {
			return Name;
		}
	}
}
