#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PFXVertex {
    vec2 uv;
} IN;

layout(location = 0) out vec4 OUT;

layout (std140) uniform PFXBlock
{ 
	uint64_t diffuse;
	uint64_t accum;
	uint64_t reveal;
} pfx;

void main() {
	sampler2D s = sampler2D(pfx.diffuse);
    OUT = vec4(texture(s, IN.uv).rgb, 1.0);
}