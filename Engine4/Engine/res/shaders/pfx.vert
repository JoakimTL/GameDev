#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec2 position;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PFXVertex {
    vec2 uv;
} OUT;

void main() {
	OUT.uv = (position + 1) * .5;
    gl_Position = vec4(position, 0, 1);
}