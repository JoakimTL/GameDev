#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PFXVertex {
    vec2 UV;
    flat vec3 color;
    flat float intensity;
    flat vec3 direction;
} IN;

layout(location = 0) out vec4 OUT;

layout (std140) uniform PFXBlock
{ 
	mat4 ipvMat;
    vec3 eyeTranslation;
	vec4 viewValues;
	uint64_t gBufferDiffuse;
	uint64_t gBufferNormal;
	uint64_t gBufferDepth;
	uint64_t gBufferLightingData;
} pfx;

#include lightCalculation.glsl

void main() {
	sampler2D diffuseTex = sampler2D(pfx.gBufferDiffuse);
	sampler2D lightingTex = sampler2D(pfx.gBufferLightingData);
	sampler2D normalTex = sampler2D(pfx.gBufferNormal);
	sampler2D depthTex = sampler2D(pfx.gBufferDepth);

	vec2 lightingData = texture(lightingTex, IN.UV).rg;
    vec3 normal = texture(normalTex, IN.UV).rgb * 2.0 - 1.0;
	vec3 worldPos = getWorldPos(IN.UV, texture(depthTex, IN.UV).r, pfx.ipvMat);

	vec3 light = calculateLightPBR(IN.color.rgb * IN.intensity,
		pfx.eyeTranslation, worldPos, -IN.direction, normal,
		texture(diffuseTex, IN.UV).rgb, lightingData.r, lightingData.g);
	
    OUT = vec4(light, 1);
}