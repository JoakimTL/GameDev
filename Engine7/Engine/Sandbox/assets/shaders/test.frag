#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec4 Position;
} IN;

layout(location = 0) out vec4 outColor;

void main(void) {
	outColor = vec4(IN.Position.x * 2, IN.Position.y * 2, 1, 1);
}