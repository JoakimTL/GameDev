using Engine.Graphics.Objects;
using System;
using System.Collections.Generic;

namespace Engine.MemLib {
	public class StoreShader {

		private readonly Dictionary<Type, Shader> shaders;

		public StoreShader() {
			shaders = new Dictionary<Type, Shader>();
		}

		public T Get<T>() where T : Shader {
			if( shaders.TryGetValue( typeof( T ), out Shader s ) )
				return s as T;
			return Set( new Lazy<T>().Value ) as T;
		}

		public Shader Set( Shader s ) {
			if( !( s is null ) )
				shaders.Add( s.GetType(), s );
			return s;
		}
	}
}
