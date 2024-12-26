using Engine.Utilities.Data.Boxing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Graphics.Objects {
	public class ShaderBundle {

		private static readonly List<IReadOnlyList<uint>> sequences = new List<IReadOnlyList<uint>>() { new List<uint>() };

		private string name;
		public string Name { get => $"Shaderbundle[{name}:{ID}]"; }
		public uint ID { get; private set; }
		private readonly Dictionary<uint, Shader> shaders;

		public ShaderBundle( string name, params ImmutableDuo<uint, Shader>[] shadersCombos ) {
			this.name = name;
			shaders = new Dictionary<uint, Shader>();

			for( int i = 0; i < shadersCombos.Length; i++ ) {
				ImmutableDuo<uint, Shader> shaderCombo = shadersCombos[ i ];
				shaders.Add( shaderCombo.ValueA, shaderCombo.ValueB );
			}

			List<uint> sortedUsecases = new List<uint>( shaders.Keys );
			sortedUsecases.Sort();

			List<uint> shaderSequence = new List<uint>();
			for( int i = 0; i < sortedUsecases.Count; i++ ) {

				shaderSequence.Add( sortedUsecases[ i ] );
				shaderSequence.Add( shaders[ sortedUsecases[ i ] ].ID );
			}

			ID = AssignID( shaderSequence );
		}

		public Shader Get( uint usecase ) {
			if( shaders.TryGetValue( usecase, out Shader r ) )
				return r;
			return null;
		}

		private uint AssignID( List<uint> shaderSequence ) {
			lock( sequences ) {
				if( shaderSequence.Count == 0 )
					return 0;

				for( int i = 0; i < sequences.Count; i++ ) {
					if( sequences[ i ].SequenceEqual( shaderSequence ) )
						return (uint) i;
				}

				sequences.Add( shaderSequence );
				return (uint) sequences.Count - 1;
			}
		}

		public override int GetHashCode() {
			return (int) ID;
		}

		public override bool Equals( object obj ) {
			if( !( obj is ShaderBundle o ) )
				return false;
			return Equals( o );
		}

		public bool Equals( ShaderBundle obj ) {
			return obj.ID == ID;
		}

		public static bool operator ==( ShaderBundle a, ShaderBundle b ) {
			if( a is null || b is null )
				return false;
			return a.Equals( b );
		}

		public static bool operator !=( ShaderBundle a, ShaderBundle b ) {
			return !( a == b );
		}

		public override string ToString() {
			return Name;
		}
	}
}
