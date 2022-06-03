#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec2 position;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PerVertex {
    vec2 uv;
} OUT;

void main() {
    gl_Position = vec4(position * 0.3, 0, 1);
	OUT.uv = gl_Position.xy;
	OUT.uv += 1;
	OUT.uv *= 0.5;
	//otex = packUint2x32(tex);
}