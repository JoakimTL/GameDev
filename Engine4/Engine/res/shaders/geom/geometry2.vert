#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec2 vPos;
layout(location = 1) in vec2 vUV;
layout(location = 2) in vec4 vCol;

layout(location = 3) in mat4 iM_mat;
layout(location = 7) in vec4 iColor;
layout(location = 8) in uint64_t iDiffuseTexture;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PerVertex {
	vec4 Position;
	vec2 UV;
	vec4 Color;
	flat uint64_t DiffuseTexture;
} OUT;

layout (std140) uniform SceneCameraBlock
{ 
	mat4 VP_mat;
	vec3 V_up;
	vec3 V_right;
} sb;

void main(void){
	mat4 MVP_mat = sb.VP_mat * iM_mat;
	
	OUT.Color = iColor * vCol;
	OUT.UV = vUV;
	OUT.Position = MVP_mat * vec4(vPos, 0.0, 1.0);
	OUT.DiffuseTexture = iDiffuseTexture;
	gl_Position = OUT.Position;
}