#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

const int NUMCASCADES = 4;

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

layout (std140) uniform DirectionalCascadeBlock
{ 
    uint64_t unfilteredCascadeTexture;
    uint64_t filteredCascadeTexture;
    uint64_t transparencyColorCascadeTexture;
    uint64_t transparencyRevealCascadeTexture;
    mat4 cascadeVP;
	vec2 textureSize;
    float cascadeDepth;
} cascades[NUMCASCADES];

const vec2 offsets[64] = vec2[](
	vec2(-0.4680406,0.26710916),
	vec2(-0.70520246,0.28895736),
	vec2(-0.5581931,0.93143094),
	vec2(0.19652879,-0.72570956),
	vec2(0.8975556,-0.091216326),
	vec2(-0.7562127,0.7889153),
	vec2(-0.24994981,-0.24084306),
	vec2(-0.83325875,0.789191),
	vec2(-0.9569137,-0.2525792),
	vec2(0.6582376,-0.23637521),
	vec2(0.8380736,0.73253655),
	vec2(0.5932094,0.93146825),
	vec2(0.8570497,0.6034075),
	vec2(-0.79622185,0.8688961),
	vec2(-0.20517218,0.07770574),
	vec2(0.37896907,0.115625024),
	vec2(0.3385322,0.21217585),
	vec2(0.15496981,-0.86006033),
	vec2(-0.77676654,0.582284),
	vec2(-0.66495335,-0.2365551),
	vec2(0.9587158,0.05726266),
	vec2(0.067530036,-0.87480235),
	vec2(-0.82691514,-0.2774434),
	vec2(-0.4261732,-0.9715252),
	vec2(0.58624303,0.23302948),
	vec2(-0.024266243,0.6933768),
	vec2(-0.36841416,-0.47796535),
	vec2(0.5237125,0.6075337),
	vec2(0.9701487,0.21471286),
	vec2(-0.16918468,-0.75366676),
	vec2(0.50803053,-0.15511298),
	vec2(0.18922019,0.5466064),
	vec2(0.36375022,-0.97858703),
	vec2(0.41735852,0.6049001),
	vec2(-0.16618764,-0.6143943),
	vec2(-0.40717924,0.82851756),
	vec2(-0.04961908,-0.4466077),
	vec2(-0.021413207,0.13437355),
	vec2(0.56313705,0.51805663),
	vec2(-0.58387065,-0.75827503),
	vec2(0.4706247,0.32353425),
	vec2(0.72713256,-0.42250347),
	vec2(0.42520833,0.26341093),
	vec2(-0.45403874,0.5423136),
	vec2(-0.092023134,-0.70970523),
	vec2(0.24355543,0.6395482),
	vec2(-0.8976985,0.12939227),
	vec2(0.47126842,0.79958093),
	vec2(-0.6187718,0.5938643),
	vec2(-0.6230097,0.2032392),
	vec2(-0.58154273,-0.90347385),
	vec2(0.2033112,0.076295614),
	vec2(-0.39741838,-0.24665236),
	vec2(-0.013535619,-0.35481358),
	vec2(-0.73030424,-0.6046022),
	vec2(-0.8103069,-0.34575915),
	vec2(0.37202573,0.048130274),
	vec2(-0.5680126,-0.29500282),
	vec2(0.6524395,0.8346205),
	vec2(-0.93106496,-0.0683831),
	vec2(-0.35763144,0.0048691034),
	vec2(0.8019414,-0.41049325),
	vec2(0.077668786,0.1669116),
	vec2(-0.2906103,0.8949673)
);

#include lightCalculation.glsl

float linearDepth(float depthSample, float zNear, float zFar)
{
    return 2 * zNear * zFar / (zFar + zNear - ( 2.0 * depthSample - 1.0) * (zFar - zNear));
}

int getCascade(float linearDepth) {
	for (int i = 0 ; i < NUMCASCADES ; i++) 
		if (linearDepth <= cascades[i].cascadeDepth)
			return i;
	return NUMCASCADES - 1;
}

float rand(vec2 co){
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

float sampleDepth(uint64_t unfilteredTexture, uint64_t filteredTexture, vec2 lUV, vec2 texOnePixel, float currentZ) {
	sampler2D unfilteredTex = sampler2D(unfilteredTexture);
	sampler2D filteredTex = sampler2D(filteredTexture);

	int n = 16;
	int index = int(rand(lUV) * (64 - n));
	float avg = 0;
	for (int i = 0; i < n; i++){
		avg += max(sign(texture(unfilteredTex, lUV + texOnePixel * offsets[i + index]).r - currentZ + 0.001), 0.0);
	}
	avg /= n;
	return avg;
}

const float EPSILON = 0.00001f;

float max3(vec3 v)
{
    return max(max(v.x, v.y), v.z);
}

void main() {
	sampler2D diffuseTex = sampler2D(pfx.gBufferDiffuse);
	sampler2D lightingTex = sampler2D(pfx.gBufferLightingData);
	sampler2D normalTex = sampler2D(pfx.gBufferNormal);
	sampler2D depthTex = sampler2D(pfx.gBufferDepth);
	
	float sampledDepth = texture(depthTex, IN.UV).r;
	float linearDepth = linearDepth(sampledDepth, pfx.viewValues.z, pfx.viewValues.w) * 0.5;
	vec2 lightingData = texture(lightingTex, IN.UV).rg;
    vec3 normal = texture(normalTex, IN.UV).rgb * 2.0 - 1.0;
	vec3 worldPosition = getWorldPos(IN.UV, sampledDepth, pfx.ipvMat);

	int cascade = getCascade(linearDepth);
	vec4 lWPos = cascades[cascade].cascadeVP * vec4(worldPosition, 1.0);
	vec2 lUV = lWPos.xy / 2 + .5;
	
	float currentZ = lWPos.z * 0.5 + 0.5;
	float lightLevel = sampleDepth(cascades[cascade].unfilteredCascadeTexture, cascades[cascade].filteredCascadeTexture, lUV, 1 / cascades[cascade].textureSize, currentZ);
	sampler2D transparencyTex = sampler2D(cascades[cascade].transparencyColorCascadeTexture);
	sampler2D revealTex = sampler2D(cascades[cascade].transparencyRevealCascadeTexture);

	vec3 diffuse = texture(diffuseTex, IN.UV).rgb;
	vec3 light = calculateLightPBR(IN.color * IN.intensity * lightLevel,
		pfx.eyeTranslation, worldPosition, -IN.direction, normal,
		diffuse, lightingData.r, lightingData.g);

    vec4 accumulation = texture(transparencyTex, lUV);
    float revealage = texture(revealTex, lUV).r;

    if (isinf(max3(abs(accumulation.rgb))))
        accumulation.rgb = vec3(accumulation.a);

    vec3 average_color = accumulation.rgb / max(accumulation.a, EPSILON);
	
    OUT = vec4(light + average_color * (1.0f - revealage) * lightLevel * diffuse, 1);
}