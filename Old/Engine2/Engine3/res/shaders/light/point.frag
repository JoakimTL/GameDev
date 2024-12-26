#version 430

layout(location = 0) out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform sampler2D uTexLightingData;
uniform samplerCube uTexShadowDepth;
uniform samplerCube uTexShadowDiffuse;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec2 lSize;
uniform float lRange;

uniform vec3 eyePos;
uniform mat4 ipvMat;

uniform vec2 viewPort;

#include lightCalculation.glsl
#include depthSampling.glsl

void main(void) {
	vec2 oUV = gl_FragCoord.xy / viewPort;
	vec3 lightingData = texture(uTexLightingData, oUV).rgb;
	float lighting = lightingData.b;
	//if (lighting == 0)
	//	discard;
	
	vec3 normal = texture(uTexNormal, oUV).rgb * 2.0 - 1.0;
	vec3 worldPos = getWorldPos(oUV, uTexDepth, ipvMat);
	vec3 lwVec = worldPos - lPos;
	float currentZ = length(lwVec);

	if (currentZ > lRange)
		discard;

	vec3 lDir = lwVec / currentZ;
	float visibility = sampleDepth(uTexShadowDepth, lDir, currentZ, lRange);
	float intensity = ( lRange * ( lRange - currentZ ) ) / ( ( currentZ + lRange ) * ( currentZ + lRange ) );
	
	vec4 pDiff = texture(uTexShadowDiffuse, lDir);

	vec3 light;
	calcLightPBR(light,  lCol.rgb * lCol.a,
		eyePos, worldPos, -lDir, normal,
		texture(uTexDiffuse, oUV).rgb, lightingData.r, lightingData.g);

	outColor = vec4(light * pDiff.rgb * visibility * intensity * pDiff.a, lighting);
	
}