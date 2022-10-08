#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in Vertex {
    vec4 Color;
    vec2 UV;
    float Thickness;
    float Edge;
	flat uint64_t FontTexture;
} IN;

layout(location = 0) out vec4 OUT;

void main() {
	sampler2D fontTex = sampler2D(IN.FontTexture);
    float distance = 1.0 - texture(fontTex, IN.UV).a;
    float g = 1.0 - smoothstep(IN.Thickness, IN.Thickness + IN.Edge, distance);
    OUT = vec4(IN.Color.rgb, IN.Color.a * g);
}