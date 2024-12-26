#version 430

in vec2 oUV;
in vec4 oPos;
in vec4 oWPos;
in vec3 oToCamera;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec4 outNormal;

uniform sampler2D reflectionTex;
uniform sampler2D refractionTex;
uniform sampler2D dudvTex;
uniform sampler2D uTexDepth;

uniform float uTime;
uniform float uTimeStep;
uniform float uWaterTiling;
uniform float uWaveIntensity;

uniform float uNear;
uniform float uFar;

float depthFromTexture(in vec2 coo) {
    float z_b = float(texture2D( uTexDepth, coo ));
	float z_n = 2.0 * z_b - 1.0;
    return 2.0 * uNear * uFar / (uFar + uNear - z_n * (uFar - uNear));
}

float depthFromFloat(in float dep) {
	float z_n = 2.0 * dep - 1.0;
    return 2.0 * uNear * uFar / (uFar + uNear - z_n * (uFar - uNear));
}

float rand(vec2 n) { 
	return fract(sin(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

float noise(vec2 p){
	vec2 ip = floor(p);
	vec2 u = fract(p);
	u = u*u*(3.0-2.0*u);
	
	float res = mix(
		mix(rand(ip),rand(ip+vec2(1.0,0.0)),u.x),
		mix(rand(ip+vec2(0.0,1.0)),rand(ip+vec2(1.0,1.0)),u.x),u.y);
	return res*res;
}

void main(void) {
	
	vec2 ndc = (oPos.xy/oPos.w) * 0.5 + 0.5;
	
	float dF = depthFromTexture(ndc);
	float dW = depthFromFloat(gl_FragCoord.z);
	
	float d = dF - dW;
	
	vec2 dudvCol = texture(dudvTex, (oUV + vec2(uTime * uTimeStep + noise(oWPos.xz + vec2(uTime, -uTime)) * uTimeStep, uTime * uTimeStep + sin(uTime * uTimeStep + noise(oWPos.zx + vec2(-uTime, uTime)) * uTimeStep))) * uWaterTiling).rg;
	vec2 offset = (dudvCol * 2.0 - 1.0) * uWaveIntensity * clamp(d / 50.0 - 0.25, 0.0, 1.0);
	vec2 dudvCol2 = texture(dudvTex, (oUV + vec2(uTime * uTimeStep + cos(uTime * uTimeStep), uTime * uTimeStep )) * uWaterTiling + offset ).rg;
	offset += (dudvCol2 * 2.0 - 1.0) * uWaveIntensity * clamp(d / 50.0 - 0.25, 0.0, 1.0);
	vec2 fraTexCoords = ndc + offset;
	fraTexCoords = clamp(fraTexCoords, 0.001, 0.999);
	vec2 fleTexCoords = vec2(ndc.x, 1.0 - ndc.y) + offset;
	fleTexCoords = clamp(fleTexCoords, 0.001, 0.999);
	vec4 refractionColor = texture(refractionTex, fraTexCoords);
	vec4 reflectionColor = texture(reflectionTex, fleTexCoords);
	
	vec3 normal = normalize(vec3(offset.x, uWaveIntensity, offset.y)) * 0.5 + 0.5;
	vec3 viewVector = normalize(oToCamera);
	float refraction = dot(viewVector, normalize(normal + vec3(0, 1, 0)));
	
	outColor = mix(reflectionColor, refractionColor, refraction);
	outColor = mix(outColor, vec4(0, 0.3, 0.5, 1.0), 0.2);
	outColor.a = clamp(d / 20.0, 0.0, 1.0);
	outNormal = vec4(normal, 1.0);
	outNormal.a = outColor.a;
}