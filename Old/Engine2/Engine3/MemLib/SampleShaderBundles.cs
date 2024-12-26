using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.LinearAlgebra;
using Engine.Utilities.Data.Boxing;
using System.Collections.Generic;

namespace Engine.MemLib {
	public class SampleShaderBundles {

		public ShaderBundle Entity3 { get; private set; }
		public ShaderBundle Particle3 { get; private set; }
		public ShaderBundle Particle2 { get; private set; }
		public ShaderBundle UI { get; private set; }
		public ShaderBundle Text { get; private set; }

		public SampleShaderBundles() {
			Entity3 = new ShaderBundle( "Entity3",
				new ImmutableDuo<uint, Shader>( 0, Mem.Shaders.Get<EntityShader>() ),
				new ImmutableDuo<uint, Shader>( 1, Mem.Shaders.Get<ShadowDirectionalShader>() ),
				new ImmutableDuo<uint, Shader>( 2, Mem.Shaders.Get<ShadowPointShader>() )
			);
			Particle3 = new ShaderBundle( "Particle3",
				new ImmutableDuo<uint, Shader>( 0, Mem.Shaders.Get<Particle3Shader>() ),
				new ImmutableDuo<uint, Shader>( 1, Mem.Shaders.Get<ShadowDirectionalParticleShader>() ),
				new ImmutableDuo<uint, Shader>( 2, Mem.Shaders.Get<ShadowPointParticleShader>() )
			);
			Particle2 = new ShaderBundle( "Particle2",
				new ImmutableDuo<uint, Shader>( 0, Mem.Shaders.Get<Particle2Shader>() )
			);
			UI = new ShaderBundle( "UI",
				new ImmutableDuo<uint, Shader>( 0, Mem.Shaders.Get<UIShader>() ),
				new ImmutableDuo<uint, Shader>( 1, Mem.Shaders.Get<UIStencilShader>() )
			);
			Text = new ShaderBundle( "Text",
				new ImmutableDuo<uint, Shader>( 0, Mem.Shaders.Get<UITextShader>() ),
				new ImmutableDuo<uint, Shader>( 1, Mem.Shaders.Get<UITextStencilShader>() )
			);
		}
	}
}
