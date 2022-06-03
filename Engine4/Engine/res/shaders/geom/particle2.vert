#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec2 vPos;
layout(location = 1) in vec2 vUV;
layout(location = 2) in vec4 vCol;

layout(location = 3) in vec2 iTranslation;
layout(location = 4) in vec2 iRotation;
layout(location = 5) in vec2 iScale;
layout(location = 6) in vec4 iColor;
layout(location = 7) in uint64_t iTexture1;
layout(location = 8) in uint64_t iTexture2;
layout(location = 9) in float iBlend;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PerVertex {
	vec4 Position;
	vec2 UV;
	vec4 Color;
	flat uint64_t Texture1;
	flat uint64_t Texture2;
	flat float Blend;
} OUT;

layout (std140) uniform SceneCameraBlock
{ 
	mat4 VP_mat;
	vec3 V_up;
	vec3 V_right;
} sb;

void main(void){
	OUT.UV = vUV;
	OUT.Texture1 = iTexture1;
	OUT.Texture2 = iTexture2;
	OUT.Blend = iBlend;
	OUT.Color = iColor;
	vec2 pos = vPos * iScale;
	vec2 rot = vec2(
		pos.x * iRotation.x - pos.y * iRotation.y,
		pos.x * iRotation.y + pos.y * iRotation.x
	);
	gl_Position = OUT.Position = sb.VP_mat * vec4(rot + iTranslation, 0.0, 1.0);
}