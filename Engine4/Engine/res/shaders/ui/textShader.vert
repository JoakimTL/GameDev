#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec2 vPos;
layout(location = 1) in vec2 vUV;
layout(location = 2) in vec4 vColor;

layout(location = 1) in vec3 iTranslation;
layout(location = 2) in vec2 iScale;
layout(location = 3) in float iRotation;
layout(location = 4) in vec4 iUV;
layout(location = 5) in vec4 iColor;
layout(location = 6) in uint64_t iFontTexture;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out Vertex {
    vec2 UV;
    vec4 Color;
	flat uint64_t FontTexture;
} OUT;

layout (std140) uniform SceneCameraBlock
{ 
	mat4 VP_mat;
	vec3 V_up;
	vec3 V_right;
} sb;

void main(void){
	OUT.UV = iUV.xy + (iUV.zw - iUV.xy) * vUV;
	OUT.Color = iColor * vColor;
	vec2 pos = vPos * iScale;
	vec2 rot = vec2(
		pos.x * iRotation.x - pos.y * iRotation.y,
		pos.x * iRotation.y + pos.y * iRotation.x
	);
	gl_Position = sb.VP_mat * vec4(iTranslation + sb.V_up * rot.y + sb.V_right * rot.x, 1.0);
}