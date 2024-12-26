#version 430

in vec2 oUV;

out vec4 outColor;

const int MAX_CASCADES = 3;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform sampler2D uTexLightingData;
uniform sampler2D uParticleDiffuse;
uniform sampler2D uParticleDepth;
uniform sampler2D uTexShadowDepth[MAX_CASCADES];
uniform sampler2D uTexShadowDiffuse[MAX_CASCADES];
uniform vec4 lCol;
uniform vec3 lDir;
uniform mat4 lvpMat[MAX_CASCADES];
uniform float uRange[MAX_CASCADES];

uniform float zNear;
uniform float zFar;

uniform vec3 eyePos;
uniform mat4 ipvMat;

#include lightCalculation.glsl
#include depthSampling.glsl

float linearDepth(float depthSample)
{
    depthSample = 2.0 * depthSample - 1.0;
    float zLinear = 2.0 * zNear * zFar / (zFar + zNear - depthSample * (zFar - zNear));
    return zLinear;
}

vec3 particle(vec4 particleDiffuse){
	vec3 worldPos = getWorldPos(oUV, uParticleDepth, ipvMat);
	float depth = linearDepth(texture(uParticleDepth, oUV).x);

	int chosenCascade = MAX_CASCADES - 1;
	for (int i = 0 ; i < MAX_CASCADES ; i++) {
		if (depth <= uRange[i]){
			chosenCascade = i;
			break;
		}
	}
	
	vec4 lWPos = lvpMat[chosenCascade] * vec4(worldPos, 1.0);
	vec2 lUV = lWPos.xy / 2 + .5;
	
	float currentZ = lWPos.z * 0.5 + 0.5;
	float visibility = sampleDepth(uTexShadowDepth[chosenCascade], lUV, currentZ);
	float d = ((dot(normalize(worldPos-eyePos), lDir) + 1) * 0.5 + 1) * 0.5;
	
	return vec3(particleDiffuse.rgb * lCol.rgb * (particleDiffuse.a * visibility * lCol.a * d));
}

void main(void) {
	vec4 particleDiffuse = texture(uParticleDiffuse, oUV);
	if (particleDiffuse.a == 0)
		discard;
	
	outColor = vec4(particle(particleDiffuse), particleDiffuse.a);
}