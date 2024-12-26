using Engine.LinearAlgebra;
using Engine.Utilities.Data.Boxing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects {
	public abstract class SceneObjectData {

		#region ID
		private static uint GetNextID() {
			lock( openID )
				return openID.Value++;
		}
		private static readonly MutableSinglet<uint> openID = new MutableSinglet<uint>( 0 );
		#endregion

		public uint ID { get; }

		public abstract ITransform TransformObject { get; }
		public Vector4 Color { get; set; } = Vector4.One;

		public SceneObjectData() {
			ID = GetNextID();
		}

		public abstract void Dispose();

	}
}
