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

layout(location = 0) out vec4 outAccumulation;
layout(location = 1) out float outReveal;

void main(void) {
	sampler2D diffuseTex = sampler2D(IN.DiffuseTexture);
	
	vec4 color = texture(diffuseTex, IN.UV) * IN.Color;

	float weight = clamp(pow(min(1.0, color.a * 10.0) + 0.01, 3.0) * 1e8 * pow(1.0 - gl_FragCoord.z * 0.9, 3.0), 1e-2, 3e3);

    outAccumulation = vec4(color.rgb * color.a, color.a) * weight;

    outReveal = color.a;
}