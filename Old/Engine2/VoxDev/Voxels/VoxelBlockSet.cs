using Engine;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Materials;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VoxDev.Voxels {
	public abstract class VoxelBlockSet {
		public string Name { get; private set; }
		public PBRMaterial Material { get; private set; }
		private readonly List<VoxelBlockType> blockTypes;
		private readonly Dictionary<string, VoxelBlockType> typesByName;
		private readonly VoxelBlockType air;
		private bool locked;

		public int TypeCount { get => blockTypes.Count; }

		public VoxelBlockSet( string name, Texture diffuse, Texture normal, Texture lighting, Texture glow ) {
			Name = name;
			Material = new PBRMaterial( $"Blockset[{Name}]", diffuse, normal, lighting, glow );
			blockTypes = new List<VoxelBlockType>();
			typesByName = new Dictionary<string, VoxelBlockType>();
			air = AddType( "Air", new Vector2b( 0, 0 ), false, false );
			LoadTypes();
			locked = true;
		}

		/// <summary>
		/// ID 0 is already occupied by air.
		/// </summary>
		protected abstract void LoadTypes();

		protected VoxelBlockType AddType( string name, Vector2b uv, bool opaque, bool solid ) {
			if( typesByName.TryGetValue( name, out VoxelBlockType type ) ) {
				Logging.Warning( "Tried to insert new block type with same name as an already present type." );
				return type;
			}
			if( locked ) {
				Logging.Warning( "Tried to insert new block type after the set has closed addition." );
				return null;
			}
			type = new VoxelBlockType( (ushort) blockTypes.Count, name, uv, opaque, solid );
			blockTypes.Add( type );
			typesByName.Add( type.Name, type );
			return type;
		}

		public VoxelBlockType GetBlockType( ushort id ) {
			if( id < blockTypes.Count )
				return blockTypes[ id ];
			return air;
		}
	}
}
