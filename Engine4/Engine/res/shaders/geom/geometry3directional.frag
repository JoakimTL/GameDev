#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec4 Position;
	vec2 UV;
	vec3 Normal;
	vec4 Color;
	float NormalMapped;
	flat uint64_t DiffuseTexture;
	flat uint64_t NormalTexture;
	flat uint64_t LightingTexture;
	flat uint64_t GlowTexture;
} IN;

void main(void) {
	sampler2D diffuseTex = sampler2D(IN.DiffuseTexture);
	if ((texture(diffuseTex, IN.UV) * IN.Color).a < 0.5)
		discard;
}