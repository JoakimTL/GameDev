#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec2 vPos;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PerVertex {
	vec4 Position;
} OUT;

void main(void){
	OUT.Position = vec4(vPos, 0.0, 1.0);
	gl_Position = OUT.Position;
}