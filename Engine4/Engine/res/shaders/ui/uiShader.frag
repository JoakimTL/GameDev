#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in Vertex {
    vec2 uv;
    vec4 color;
} IN;

layout(location = 0) out vec4 OUT;

void main() {
    OUT = IN.color;
}