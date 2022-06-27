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

float smoothClamp(float x, float a, float b)
{
    return smoothstep(0., 1., (x - a)/(b - a))*(b - a) + a;
}

void main() {
	sampler2D fontTex = sampler2D(IN.FontTexture);
    //float g = smoothClamp((texture(fontTex, IN.UV).a * 2 - 1) * 4, 0, 1) * 2;//max(sign(texture(fontTex, IN.UV).a - 0.5), 0.0) * 2;
    float distance = 1.0 - texture(fontTex, IN.UV).a;
    float g = 1.0 - smoothstep(IN.Thickness, IN.Thickness + IN.Edge, distance);
    OUT = vec4(IN.Color.rgb, IN.Color.a * g);
}