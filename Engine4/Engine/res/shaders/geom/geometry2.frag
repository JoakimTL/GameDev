#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec4 Position;
	vec2 UV;
	vec4 Color;
	flat uint64_t DiffuseTexture;
} IN;

layout(location = 0) out vec4 outColor;

void main(void) {
	sampler2D diffuseTex = sampler2D(IN.DiffuseTexture);
	outColor = texture(diffuseTex, IN.UV) * IN.Color;
	if (outColor.a < 0.5)
		discard;
}