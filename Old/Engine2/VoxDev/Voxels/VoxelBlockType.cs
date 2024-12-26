using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDev.Voxels {
	public class VoxelBlockType {

		public ushort ID { get; private set; }
		public string Name { get; private set; }
		public Vector2b TextureUV { get; private set; } //make changeable for each face?
		public bool Opaque { get; private set; }
		/// <summary>
		/// Whether or not the block is collidable
		/// </summary>
		public bool Solid { get; private set; }

		internal VoxelBlockType( ushort id, string name, Vector2b texUV, bool opaque, bool solid ) {
			ID = id;
			Name = name;
			TextureUV = texUV;
			Opaque = opaque;
			Solid = solid;
		}

	}
}
