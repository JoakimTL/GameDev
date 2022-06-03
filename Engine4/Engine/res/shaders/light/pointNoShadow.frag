#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PFXVertex {
    flat vec3 color;
    flat float intensity;
    flat float radius;
    flat vec3 translation;
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
	vec2 uv = gl_FragCoord.xy / pfx.viewValues.xy;

	vec3 worldPos = getWorldPos(uv, texture(depthTex, uv).r, pfx.ipvMat);
	vec3 fromLightToWorldPos = worldPos - IN.translation;
	float distanceFromLight = length(fromLightToWorldPos);
	if (distanceFromLight > IN.radius)
		discard;

	vec2 lightingData = texture(lightingTex, uv).rg;
    vec3 normal = texture(normalTex, uv).rgb * 2.0 - 1.0;
	//float attenuation = 1 / ((distanceFromLight + 1) * (distanceFromLight + 1))*(IN.radius - distanceFromLight)/IN.radius; //realistic
	float attenuation = 1 - (distanceFromLight*distanceFromLight)/(IN.radius*IN.radius); //non-realistic

	vec3 light = calculateLightPBR(IN.color * IN.intensity * attenuation,
		pfx.eyeTranslation, worldPos, -fromLightToWorldPos / distanceFromLight, normal,
		texture(diffuseTex, uv).rgb, lightingData.r, lightingData.g);
	
    OUT = vec4(light, 1);
}