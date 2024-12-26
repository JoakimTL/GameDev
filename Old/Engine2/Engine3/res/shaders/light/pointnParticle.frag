#version 430

layout(location = 0) out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform sampler2D uTexLightingData;
uniform sampler2D uParticleDiffuse;
uniform sampler2D uParticleDepth;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec2 lSize;
uniform float lRange;

uniform vec3 eyePos;
uniform mat4 ipvMat;

uniform vec2 viewPort;

#include lightCalculation.glsl

void main(void) {
	vec2 oUV = gl_FragCoord.xy / viewPort;
	vec4 particleDiffuse = texture(uParticleDiffuse, oUV);
	if (particleDiffuse.a == 0)
		discard;

	vec3 worldPos = getWorldPos(oUV, uParticleDepth, ipvMat);
	
	vec3 lwVec = worldPos - lPos;
	float currentZ = length(lwVec);
	vec3 lDir = lwVec / currentZ;
	
	float d = ((dot(normalize(worldPos-eyePos), lDir) + 1) * 0.5 + 1) * 0.5;
	float intensity = ( lRange * ( lRange - currentZ ) ) / ( ( currentZ + lRange ) * ( currentZ + lRange ) );
	
	outColor = vec4(particleDiffuse.rgb * lCol.rgb * (particleDiffuse.a * lCol.a * d * intensity), particleDiffuse.a);
}