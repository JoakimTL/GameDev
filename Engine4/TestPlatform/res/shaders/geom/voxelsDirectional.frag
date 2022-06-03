#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec3 normal;
	vec4 diffuseColor;
	vec4 glowColor;
	flat float metallic;
	flat float roughness;
} IN;

void main(void) {
}