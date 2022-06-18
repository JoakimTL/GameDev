#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in Vertex {
    vec4 UV;
    vec4 Color;
	flat uint64_t FontTexture;
} IN;

layout(location = 0) out vec4 OUT;

void main() {
	sampler2D fontTex = sampler2D(IN.FontTexture);
    OUT = texture(fontTex, IN.UV) * IN.color;
}