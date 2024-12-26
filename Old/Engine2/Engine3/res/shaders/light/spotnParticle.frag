#version 430

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform sampler2D uTexLightingData;
uniform sampler2D uParticleDiffuse;
uniform sampler2D uParticleDepth;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec2 lAtt;
uniform float lRange;
uniform vec3 lDir;
uniform float lCut;

uniform vec3 eyePos;
uniform mat4 ipvMat;

uniform vec2 viewPort;

#include lightCalculation.glsl

void main(void) {
	vec2 oUV = gl_FragCoord.xy / viewPort;
	vec4 particleDiffuse = texture(uParticleDiffuse, oUV);
	if (particleDiffuse.a == 0)
		discard;
		
	vec3 worldPos = getWorldPos(oUV, uTexDepth, ipvMat);
	vec3 lightDir = worldPos - lPos;
	float distanceToPoint = length(lightDir);

	if (distanceToPoint > lRange)
		discard;

	lightDir = normalize(lightDir);
	
	float spotFactor = dot(lightDir, lDir);
	
	outColor = vec4(0);
	
	if (spotFactor > lCut) {
		float d = ((dot(normalize(worldPos-eyePos), lDir) + 1) * 0.5 + 1) * 0.5;
		float intensity = ( lRange * ( lRange - distanceToPoint ) ) / ( ( distanceToPoint + lRange ) * ( distanceToPoint + lRange ) );
		
		outColor = vec4(particleDiffuse.rgb * lCol.rgb * (particleDiffuse.a * lCol.a * d * intensity), 1.0);
		outColor *= (1 - (1 - spotFactor) / (1 - lCut));
	}
}