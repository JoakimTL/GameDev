#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec3 vPos;
layout(location = 1) in vec2 vUV;
layout(location = 2) in vec3 vNor;
layout(location = 3) in vec4 vCol;

layout(location = 4) in mat4 iM_mat;
layout(location = 8) in vec4 iColor;
layout(location = 9) in float iNormalMapped;
layout(location = 10) in uint iDiffuseTexture;
layout(location = 11) in uint iNormalTexture;
layout(location = 12) in uint iLightingTexture;
layout(location = 13) in uint iGlowTexture;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PerVertex {
	vec4 Position;
	vec2 UV;
	vec3 Normal;
	vec4 Color;
	float NormalMapped;
	flat uint DiffuseTexture;
	flat uint NormalTexture;
	flat uint LightingTexture;
	flat uint GlowTexture;
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
	OUT.Normal = normalize((iM_mat * vec4(vNor, 0.0)).xyz);
	OUT.UV = vUV;
	OUT.Position = MVP_mat * vec4(vPos, 1.0);
	OUT.NormalMapped = iNormalMapped;
	OUT.DiffuseTexture = iDiffuseTexture;
	OUT.NormalTexture = iNormalTexture;
	OUT.LightingTexture = iLightingTexture;
	OUT.GlowTexture = iGlowTexture;
	gl_Position = OUT.Position;
}