#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
    vec2 uv;
} IN;

//layout(location = 1) in flat uint64_t otex;

layout(location = 0) out vec4 OUT;

void main() {
	//sampler2D s = sampler2D(otex);
    OUT = vec4(1,0,0,0.33);//texture(s, IN.uv);
}