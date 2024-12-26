#version 430

in vec2 oUV;

layout(location = 0) out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform sampler2D uTexShadowDepth;
uniform sampler2D uTexLightingData;
uniform vec4 lCol;
uniform vec3 lDir;
uniform vec2 lSize;
uniform float lArea;
uniform mat4 lvpMat;

uniform vec3 eyePos;
uniform mat4 ipvMat;

//const int CSMCount = 4;
//uniform sampler2D shadowTextures[];
//Linear mapping of shadow buffer zones?
//Quadratic?? Meaning the depth at which another map is used for reference.

#include lightCalculation.glsl
#include depthSampling.glsl

void main(void) {
	vec3 lightingData = texture(uTexLightingData, oUV).rgb;
	float lighting = lightingData.b;
	if (lighting == 0)
		discard;
	
	vec3 normal = texture(uTexNormal, oUV).rgb * 2.0 - 1.0;
	vec3 worldPos = getWorldPos(oUV, uTexDepth, ipvMat);
	vec4 lCalcPos = lvpMat * vec4(worldPos, 1.0);
	vec2 lUV = lCalcPos.xy / 2 + .5;
	
	float currentZ = lCalcPos.z * 0.5 + 0.5;
	float visibility = 1;
	
	visibility -= (1 - sampleDepth(uTexShadowDepth, lUV, currentZ));
	
	//makes a smooth transition from shadowmapped area to non-shadowmapped area
	float s = (-sign(currentZ - 1) + 1) / 2;
	visibility = (visibility - 1) * s + 1;
	visibility += smoothstep(lArea - lArea * .2, lArea, length(cross(worldPos - eyePos, lDir)));
	visibility = min(1, visibility);
	
	vec3 light;
	calcLightPBR(light, lCol.rgb * lCol.a,
		eyePos, worldPos, -lDir, normal,
		texture(uTexDiffuse, oUV).rgb, lightingData.r, lightingData.g);
	
	outColor = vec4(light * visibility, 1.0);
}