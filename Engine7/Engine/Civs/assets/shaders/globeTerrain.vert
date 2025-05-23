#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec3 vPos;
layout(location = 1) in vec2 vUv;
layout(location = 2) in vec3 vNor;
layout(location = 3) in vec4 vColor;
layout(location = 4) in mat4 iM_mat;
layout(location = 8) in vec4 iColor;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PerVertex {
	vec4 Position;
	vec4 Color;
} OUT;

layout (std140) uniform SceneCameraBlock
{ 
	mat4 VP_mat;
	vec3 V_up;
	vec3 V_right;
} sb;

void main(void){
	mat4 MVP_mat = sb.VP_mat;

	OUT.Position = MVP_mat * vec4(vPos.xyz, 1.0);
	OUT.Color = vColor;
	gl_Position = OUT.Position;
}