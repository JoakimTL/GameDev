#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform float uNearZ;
uniform float uFarZ;
uniform float uNorThreshold;
uniform float uDepThreshold;
uniform float uRange;
uniform vec3 uColor;
uniform vec2 uOutputSize;

float depthFromFloat(float dep) {
	float z_n = 2.0 * dep - 1.0;
	return 2.0 * uNearZ * uFarZ / (uFarZ + uNearZ - z_n * (uFarZ - uNearZ));
}

float sampleValue(vec2 depthsA, vec3 normalsA, vec2 bUV){
	float linearDepthA = depthFromFloat(depthsA.x);
	vec2 depthsB = texture(uTexDepth, bUV).rg;
	float linearDepthB = depthFromFloat(depthsB.x);

	vec3 normalsB = texture(uTexNormal, bUV).xyz * 2.0 - 1.0;
	
	float varianceA = depthsA.y - depthsA.x * depthsA.x;
	float varianceB = depthsB.y - depthsB.x * depthsB.x;
	
	float depAlpha = floor(abs(linearDepthA - linearDepthB) - uDepThreshold * (linearDepthA * linearDepthA * varianceA + linearDepthB * linearDepthB * varianceB));
	depAlpha = max(depAlpha, 0.0);

	float norAlpha = sign((1.0 - dot(normalsA, normalsB)) - uNorThreshold);
	norAlpha = max(norAlpha, 0.0);	
	
	return depAlpha + norAlpha;
}

void main(void) {
	if (texture2D(uTexDepth, oUV).r == 1.0)
		discard;
	float a = 0;

	vec2 xy;
	vec2 depth = texture(uTexDepth, oUV).rg;
	vec3 normal = texture(uTexNormal, oUV).rgb * 2.0 - 1.0;
	
	xy = vec2(0.0, -uOutputSize.y);
	a += sampleValue(depth, normal, oUV + xy);

	xy = vec2(0.0, uOutputSize.y);
	a += sampleValue(depth, normal, oUV + xy);

	xy = vec2(-uOutputSize.x, 0.0);
	a += sampleValue(depth, normal, oUV + xy);

	xy = vec2(uOutputSize.x, 0.0);
	a += sampleValue(depth, normal, oUV + xy);
	
	outColor = vec4(uColor, max(min(a, 1.0) - depthFromFloat(depth.r) / uRange, 0.0));
}