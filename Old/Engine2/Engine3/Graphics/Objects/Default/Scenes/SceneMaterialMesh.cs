﻿using Engine.Graphics.Objects.Default.SceneObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Scenes {
	public class SceneMaterialMesh<T> : Scene<T> where T : SceneObjectData {

		public SceneMaterialMesh( string name, GLWindow window ) : base( name, window ) { }

		protected override int Comparator( SceneObject<T> ra, SceneObject<T> rb ) {
			RenderableSum a = ra.Sum;
			RenderableSum b = rb.Sum;
			if( a.ShaderID == b.ShaderID ) {
				if( a.MaterialID == b.MaterialID ) {
					if( a.MeshID == b.MeshID ) {
						return 0;
					} else {
						if( a.MeshID < b.MeshID )
							return -1;
						return 1;
					}
				} else {
					if( a.MaterialID < b.MaterialID )
						return -1;
					return 1;
				}
			} else {
				if( a.ShaderID < b.ShaderID )
					return -1;
				return 1;
			}
		}
	}
}