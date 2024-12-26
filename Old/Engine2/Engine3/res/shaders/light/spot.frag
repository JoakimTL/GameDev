#version 430

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform sampler2D uTexLightingData;
uniform samplerCube uTexShadowDepth;
uniform samplerCube uTexShadowDiffuse;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec2 lAtt;
uniform float lRange;
uniform vec3 lDir;
uniform float lCut;
uniform vec2 lSize;

uniform float zNear;
uniform float zFar;

uniform vec3 eyePos;
uniform mat4 ipvMat;

uniform vec2 viewPort;

#include lightCalculation.glsl
#include depthSampling.glsl

void main(void) {
	vec2 oUV = gl_FragCoord.xy / viewPort;
	vec3 lightingData = texture(uTexLightingData, oUV).rgb;
	float lighting = lightingData.b;
	if (lighting == 0)
		discard;
		
	vec3 worldPos = getWorldPos(oUV, uTexDepth, ipvMat);
	vec3 lightDir = worldPos - lPos;
	float distanceToPoint = length(lightDir);

	if (distanceToPoint > lRange)
		discard;

	lightDir = normalize(lightDir);
	float spotFactor = dot(lightDir, lDir);
	
	outColor = vec4(0.0);
	
	if (spotFactor > lCut) {
		vec3 normal = texture(uTexNormal, oUV).rgb * 2.0 - 1.0;
		float visibility = sampleDepth(uTexShadowDepth, lightDir, distanceToPoint, lRange);
		float intensity = ( lRange * ( lRange - distanceToPoint ) ) / ( ( distanceToPoint + lRange ) * ( distanceToPoint + lRange ) );
		
		vec4 pDiff = texture(uTexShadowDiffuse, lDir);
		
		vec3 light;
		calcLightPBR(light, lCol.rgb * lCol.a,
			eyePos, worldPos, -lDir, normal,
			texture(uTexDiffuse, oUV).rgb, lightingData.r, lightingData.g);
		outColor = vec4(light * pDiff.rgb * visibility * intensity * pDiff.a, 1.0);

		outColor *= (1 - (1 - spotFactor) / (1 - lCut));
	}
}