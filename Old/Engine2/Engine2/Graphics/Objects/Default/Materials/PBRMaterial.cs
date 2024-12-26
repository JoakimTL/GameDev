using Engine.LMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Materials {
	public class PBRMaterial : Material{

		public Property Properties { get; private set; }

		public PBRMaterial( string name ) : base( name ) {
			Properties = new Property();
			shaderBind = PBRApplication;
		}

		private void PBRApplication( Shader s ) {
			s.Set( "uAmbient", Properties.IsAffectedByAmbient() );
			s.Set( "uLighting", Properties.IsAffectedByLights() );
			s.Set( "uMetallic", Properties.GetMetallic() );
			s.Set( "uRoughness", Properties.GetRoughness() );
			s.Set( "uDiffuseIntensity", Properties.GetDiffuseIntensity() );
			s.Set( "uColor", Properties.GetColor() );
			s.Set( "uNormalMapped", Properties.IsNormalMapped() );
		}

		public class Property {

			private float diffuseIntensity;
			private float metallic;
			private float roughness;
			private Vector4 materialColor;
			private bool ambientLight;
			private bool lighting;
			private bool normalMapped;

			public Property() {
				diffuseIntensity = 1;
				metallic = 0.0f;
				roughness = 0.1f;
				materialColor = new Vector4( 1, 1, 1, 1 );
				ambientLight = true;
				lighting = true;
				normalMapped = false;
			}

			public float GetDiffuseIntensity() => diffuseIntensity;
			public float GetMetallic() => metallic;
			public float GetRoughness() => roughness;
			public Vector4 GetColor() => materialColor;
			public bool IsAffectedByAmbient() => ambientLight;
			public int AffectedByAmbient() => ambientLight ? 1 : 0;
			public bool IsAffectedByLights() => lighting;
			public int AffectedByLights() => lighting ? 1 : 0;
			public bool IsNormalMapped() => normalMapped;

			public Property SetDiffuseIntensity( float f ) {
				diffuseIntensity = f;
				return this;
			}
			public Property SetMetallic( float f ) {
				if( f >= 0.0f )
					metallic = f;
				return this;
			}
			public Property SetRoughness( float f ) {
				if( f >= 0.1f )
					roughness = f;
				return this;
			}
			public Property SetColor( Vector4 c ) {
				materialColor = c;
				return this;
			}
			public Property SetAffectedByAmbient( bool b ) {
				ambientLight = b;
				return this;
			}
			public Property SetAffectedByLights( bool b ) {
				lighting = b;
				return this;
			}
			public Property SetNormalMapped( bool b ) {
				normalMapped = b;
				return this;
			}

		}
	}
}
