#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec3 vPos;
layout(location = 1) in vec2 vUV;
layout(location = 2) in vec3 vNor;
layout(location = 3) in vec4 vCol;

layout(location = 4) in vec3 iTranslation;
layout(location = 5) in vec3 iVelocity;
layout(location = 6) in float iRadius;
layout(location = 7) in vec4 iColor;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PerVertex {
	vec4 Position;
	vec2 UV;
	vec4 Color;
} OUT;

layout (std140) uniform SceneCameraBlock
{ 
	mat4 VP_mat;
	vec3 V_up;
	vec3 V_right;
} sb;

void main(void){
	OUT.UV = vUV;
	OUT.Color = iColor * vCol;
	gl_Position = OUT.Position = sb.VP_mat * vec4(iTranslation + vPos * iRadius, 1.0);
}