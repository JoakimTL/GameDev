#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec4 Position;
	vec2 UV;
	vec4 Color;
} IN;

layout(location = 0) out vec4 outColor;

void main(void) {
	outColor = IN.Color;
}