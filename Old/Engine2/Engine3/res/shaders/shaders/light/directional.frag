#version 430

in vec2 oUV;

out vec4 outColor;

const int MAX_CASCADES = 4;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform sampler2D uTexLightingData;
uniform sampler2D uTexShadowDepth[MAX_CASCADES];
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

void main(void) {
	vec3 lightingData = texture(uTexLightingData, oUV).rgb;
	float lighting = lightingData.b;
	if (lighting == 0)
		discard;
	
	vec3 normal = texture(uTexNormal, oUV).rgb * 2.0 - 1.0;
	vec3 worldPos = getWorldPos(oUV, uTexDepth, ipvMat);
	float depth = linearDepth(texture(uTexDepth, oUV).x);

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
	
	vec3 light;
	calcLightPBR(light, lCol.rgb * lCol.a,
		eyePos, worldPos, -lDir, normal,
		texture(uTexDiffuse, oUV).rgb, lightingData.r, lightingData.g);
	
	outColor = vec4(light * visibility, 1.0);
}