namespace Engine.Graphics.Objects.Default.Shaders {
	public static class ShaderSources {

		#region ambient
		public const string v_ambient = @"
#version 430

in vec2 vPos;

out vec2 oUV;

void main(void){
	oUV = vPos / 2 + .5f;
	gl_Position = vec4(vPos, 0, 1);
}";
		public const string f_ambient = @"
#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform vec3 lCol;

void main(void) {
	
	float ambient = texture(uTexNormal, oUV).w + (1 - texture(uTexNormal, oUV).w);
	
	outColor = vec4((texture(uTexDiffuse, oUV).xyz) * ambient * (vec3(1) - lCol * texture(uTexDiffuse, oUV).a), 1);
}";
		#endregion

		#region bloom
		public const string f_bloom = @"
#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform float threshold;
uniform vec3 colorWeight;
uniform float intensity;

void main(void) {
	
	vec4 c = texture(uTexDiffuse, oUV);
	vec3 rgbWeighted = (c.rgb * colorWeight);
	float t = max((rgbWeighted.r + rgbWeighted.g + rgbWeighted.b) - threshold, 0);
	outColor = c * t * intensity;
	
}";
		#endregion

		#region blurs
		public const string f_blurDefault = @"
#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform vec2 tSize;
uniform vec2 uBlur;

const float[7] list = float[] (
	1.0 / 64,
	6.0 / 64,
	15.0 / 64,
	20.0 / 64,
	15.0 / 64,
	6.0 / 64,
	1.0 / 64
);

void main(void) {
	
	vec2 rSize = uBlur / tSize;
	vec4 avg = vec4(0);
	
	avg += texture(uTexDiffuse, oUV + vec2(-3.0) * rSize) * list[0];
	avg += texture(uTexDiffuse, oUV + vec2(-2.0) * rSize) * list[1];
	avg += texture(uTexDiffuse, oUV + vec2(-1.0) * rSize) * list[2];
	avg += texture(uTexDiffuse, oUV + vec2(0.0) * rSize) * list[3];
	avg += texture(uTexDiffuse, oUV + vec2(1.0) * rSize) * list[4];
	avg += texture(uTexDiffuse, oUV + vec2(2.0) * rSize) * list[5];
	avg += texture(uTexDiffuse, oUV + vec2(3.0) * rSize) * list[6];
	
	outColor = avg;
	
}";

		public const string f_blurShadow = @"
#version 430

in vec2 oUV;

out vec2 outColor;

uniform sampler2D uTexDiffuse;
uniform vec2 tSize;
uniform vec2 uBlur;

const float[7] list = float[] (
	1.0 / 64,
	6.0 / 64,
	15.0 / 64,
	20.0 / 64,
	15.0 / 64,
	6.0 / 64,
	1.0 / 64
);

void main(void) {
	
	vec2 rSize = uBlur / tSize;
	vec2 avg = vec2(0);
	
	avg += texture(uTexDiffuse, oUV + vec2(-3.0) * rSize).rg * list[0];
	avg += texture(uTexDiffuse, oUV + vec2(-2.0) * rSize).rg * list[1];
	avg += texture(uTexDiffuse, oUV + vec2(-1.0) * rSize).rg * list[2];
	avg += texture(uTexDiffuse, oUV + vec2(0.0) * rSize).rg * list[3];
	avg += texture(uTexDiffuse, oUV + vec2(1.0) * rSize).rg * list[4];
	avg += texture(uTexDiffuse, oUV + vec2(2.0) * rSize).rg * list[5];
	avg += texture(uTexDiffuse, oUV + vec2(3.0) * rSize).rg * list[6];
	
	outColor = avg;
	
}";
		#endregion

		#region cel
		public const string f_cel = @"
#version 430

in vec2 oUV;

layout(location = 0) out vec4 outColor;

uniform sampler2D uTexture;
uniform float uSteps;

void main(void) {
	vec4 col = texture(uTexture, oUV);
	float inverse = 1 / uSteps;
	outColor = vec4(
		round(col.x * uSteps) * inverse, 
		round(col.y * uSteps) * inverse, 
		round(col.z * uSteps) * inverse, 
		col.w
	);
}";
		#endregion

		#region fog
		public const string f_fog = @"
#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDepth;

uniform float zNear;
uniform float zFar;
uniform float uFOV;
uniform float uDepth;
uniform vec3 uColor;

float depth(in vec2 coo) {
    float z_b = float(texture2D( uTexDepth, coo ));
	float z_n = 2.0 * z_b - 1.0;
    return 2.0 * zNear * zFar / (zFar + zNear - z_n * (zFar - zNear));
}

void main(void) {
	float depthSample = depth( oUV );
	outColor = vec4(uColor, (depthSample * depthSample * depthSample) / (uDepth * uDepth * uDepth));
}
";
		#endregion

		#region outline
		public const string f_outline = @"
#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform vec2 uOffset[9];

uniform float zNear;
uniform float zFar;
uniform float bias;
uniform float uDepth;

float depth(in vec2 coo) {
    vec4 depth = texture2D( uTexDepth, coo );
	//return float(depth);
	return (2 * zNear) / ( zFar + zNear - float( depth ) * ( zFar - zNear ) );
}

void main(void) {
	
	vec3 normal0 = (texture(uTexNormal, oUV + uOffset[0]).xyz - .5) * 2;
	vec3 normal1 = (texture(uTexNormal, oUV + uOffset[1]).xyz - .5) * 2;
	vec3 normal2 = (texture(uTexNormal, oUV + uOffset[2]).xyz - .5) * 2;
	vec3 normal3 = (texture(uTexNormal, oUV + uOffset[3]).xyz - .5) * 2;
	vec3 normal4 = (texture(uTexNormal, oUV).xyz - .5) * 2;
	vec3 normal5 = (texture(uTexNormal, oUV + uOffset[5]).xyz - .5) * 2;
	vec3 normal6 = (texture(uTexNormal, oUV + uOffset[6]).xyz - .5) * 2;
	vec3 normal7 = (texture(uTexNormal, oUV + uOffset[7]).xyz - .5) * 2;
	vec3 normal8 = (texture(uTexNormal, oUV + uOffset[8]).xyz - .5) * 2;

	float sample0 = depth( oUV + uOffset[0] );
	float sample1 = depth( oUV + uOffset[1] );
	float sample2 = depth( oUV + uOffset[2] );
	float sample3 = depth( oUV + uOffset[3] );
	float sample5 = depth( oUV + uOffset[5] );
	float sample6 = depth( oUV + uOffset[6] );
	float sample7 = depth( oUV + uOffset[7] );
	float sample8 = depth( oUV + uOffset[8] );
    
	// The result fragment sample matrix is as below, where x is the current fragment(4)
    // 0 1 2
    // 3 x 5
    // 6 7 8
    
    float areaMx = max( sample0, max( sample1, max( sample2, max( sample3, max( sample5, max( sample6, max( sample7, sample8 ) ) ) ) ) ) );
    float areaMn = min( sample0, min( sample1, min( sample2, min( sample3, min( sample5, min( sample6, min( sample7, sample8 ) ) ) ) ) ) );
    
	float n0 = abs(dot(normal0, normal4));
	float n1 = abs(dot(normal1, normal4));
	float n2 = abs(dot(normal2, normal4));
	float n3 = abs(dot(normal3, normal4));
	float n5 = abs(dot(normal5, normal4));
	float n6 = abs(dot(normal6, normal4));
	float n7 = abs(dot(normal7, normal4));
	float n8 = abs(dot(normal8, normal4));
	float n = 1 - min( n0, min( n1, min( n2, min( n3, min( n5, min( n6, min( n7, n8 ) ) ) ) ) ) ) - bias * 5;

	float t = sign(max(min(1, (areaMx * areaMx * areaMx - areaMn * areaMn * areaMn)) - bias, 0));

	float a = ( uDepth * sign(t + n) ) / (areaMn * areaMn);

	if (a < 0.1)
		discard;

	outColor = vec4(0, 0, 0, a);
}
";
		#endregion

		#region geometry
		#region particle
		public const string v_geoParticle = @"
#version 430

in vec2 vPos;

in mat4 iM_Mat;
in vec4 iTex;
in vec4 iColor;
in float iBlend;

out vec4 oPos;
out vec4 oTex;
out vec4 oColor;
out float oBlend;

uniform mat4 uVP_mat;
uniform int rows;

uniform vec4 clipPlane;

void main(void){
	vec2 tex = (vPos + vec2(.5, .5));
	tex.y = -(1 - tex.y);
	tex /= rows;
	oTex = vec4(tex + iTex.xy, tex + iTex.zw);
	oColor = iColor;
	oBlend = iBlend;
	oPos = iM_Mat * vec4(vPos, 0, 1);
	gl_ClipDistance[0] = dot(clipPlane, oPos);
	gl_Position = oPos = uVP_mat * oPos;
}";
		public const string f_geoParticle2 = @"
#version 430

in vec4 oPos;
in vec4 oTex;
in vec4 oColor;
in float oBlend;

uniform sampler2D uTexture;

layout(location = 0) out vec4 outParticles;

void main(void){
	outParticles = mix(texture(uTexture, oTex.xy), texture(uTexture, oTex.zw), oBlend) * oColor;
}";
		public const string f_geoParticle3 = @"
#version 430

in vec4 oPos;
in vec4 oTex;
in vec4 oColor;
in float oBlend;

uniform sampler2D uTexture;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec4 outNormal;
layout(location = 2) out vec2 outSpecular;
layout(location = 3) out vec4 outGlow;
layout(location = 4) out vec4 outParticles;

void main(void){
	outColor = vec4(0);
	outNormal = vec4(0);
	outSpecular = vec2(0);
	outGlow = vec4(0);
	outParticles = mix(texture(uTexture, oTex.xy), texture(uTexture, oTex.zw), oBlend) * oColor;
}";
		#endregion
		#region entity
		public const string v_geoEntity = @"
#version 430

in vec3 vPos;
in vec3 vCol;
in vec3 vNor;
in vec2 vUV;

out vec4 oCol;
out vec3 oNorT;
out vec4 oPos;
out vec2 oUV;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;
uniform mat4 uVP_mat;

uniform float ambient;

uniform vec4 clipPlane;

void main(void){
	oCol = vec4(vCol, ambient);
	oNorT = normalize((uM_mat * vec4(vNor, 0)).xyz);
	oPos = vec4(vPos, 1);
	oUV = vUV;
	gl_ClipDistance[0] = dot(clipPlane, uM_mat * oPos);
	gl_Position = uMVP_mat * oPos;
	oPos = gl_Position;
}";
		public const string f_geoEntity = @"
#version 430

in vec4 oPos;
in vec4 oCol;
in vec3 oNorT;
in vec2 oUV;

uniform vec4 uColor;
uniform float sPower;
uniform float sIntensity;
uniform float lighting;
uniform float uNormalMapped;

uniform sampler2D cTexture;
uniform sampler2D nTexture;
uniform sampler2D sTexture;
uniform sampler2D gTexture;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec4 outNormal;
layout(location = 2) out vec2 outSpecular;
layout(location = 3) out vec4 outGlow;
layout(location = 4) out vec4 outParticles;
layout(location = 5) out float outRefraction;

void main(void) {
	outColor = vec4(texture(cTexture, oUV).rgb, 1) * oCol * uColor;
	outNormal = vec4(normalize(oNorT + (texture(nTexture, oUV).rgb * 2 - 1) * uNormalMapped) / 2 + .5, lighting);
	outSpecular = vec2(sIntensity, sPower) * texture(sTexture, oUV).r;
	outGlow = texture(gTexture, oUV);
	outParticles = vec4(0,0,0,0);
	outRefraction = 0;
}";
		#endregion
		#region shadow
		public const string v_geoShadow = @"
#version 430

in vec3 vPos;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;
uniform mat4 uVP_mat;

void main(void){
	gl_Position = uMVP_mat * vec4(vPos, 1);
}";
		public const string f_geoShadow = @"
#version 430

void main(void) {
}";
		#region directional
		public const string v_geoShadowDirectional = @"
#version 430

in vec3 vPos;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;
uniform mat4 uVP_mat;

void main(void){
	gl_Position = uMVP_mat * vec4(vPos, 1);
}";
		public const string f_geoShadowDirectional = @"
#version 430

layout(location = 0) out vec2 depthData;

void main(void) {
	float depth = gl_FragCoord.z;
	float dx = dFdx(depth);
	float dy = dFdy(depth);
	float momentZ = depth * depth + 0.25 * (dx * dx + dy * dy);
	depthData = vec2(depth, momentZ);
}";
		#endregion
		#region point
		public const string v_geoShadowPoint = @"
#version 430

in vec3 vPos;

out vec3 oPos;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;
uniform mat4 uVP_mat;

void main(void){
	vec4 pos = vec4(vPos, 1);
	gl_Position = uMVP_mat * pos;
	oPos = (uM_mat * pos).xyz;
}";
		public const string f_geoShadowPoint = @"
#version 430

in vec3 oPos;

out float outColor;

uniform vec3 lightPos;

void main(void) {
	outColor = length(lightPos - oPos);
}";
		#endregion
		#endregion
		#region water
		public const string v_geoWater = @"
#version 430

in vec3 vPos;
in vec2 vUV;

out vec2 oUV;
out vec4 oPos;
out vec4 oWPos;
out vec3 oToCamera;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;
uniform mat4 uVP_mat;
uniform vec3 uCameraTranslation;

void main(void){
	oUV = vUV;
	gl_Position = oPos = uMVP_mat * vec4(vPos, 1);
	oWPos = uM_mat *  vec4(vPos, 1);
	oToCamera = uCameraTranslation - oWPos.xyz;
}";
		public const string f_geoWater = @"
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
}";
		#endregion
		#endregion

		#region gui
		public const string v_gui = @"
#version 430

in vec3 vPos;
in vec3 vCol;
in vec2 vUV;

out vec4 oCol;
out vec4 oPos;
out vec2 oUV;

uniform mat4 uMVP_mat;

void main(void){
	oCol = vec4(vCol, 1);
	oPos = vec4(vPos, 1);
	oUV = vUV;
	gl_Position = uMVP_mat * oPos;
}";
		public const string f_gui = @"
#version 430

in vec4 oCol;
in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexture;
uniform vec4 uColor;

void main(void) {
	
	outColor = texture(uTexture, oUV) * oCol * uColor;
	
}";
		#endregion

		#region light
		public const string f_lAmbient = @"
#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexLightingData;
uniform sampler2D gloTex;
uniform vec3 uCol;

void main(void) {
	
	vec4 glo = texture(gloTex, oUV);
	outColor = vec4((texture(uTexDiffuse, oUV).xyz) * (vec3(1)-uCol * texture(uTexDiffuse, oUV).a) + glo.rgb * glo.a, 1);

}";
		#region directional
		public const string f_lDirectional = @"
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

void calcLight(out vec4 diffuseColor, out vec4 specularColor, vec4 col, vec3 direction, vec3 normal, vec3 worldPos, float specularIntensity, float specularPower) {
	float diffuseFactor = max(dot(normal, -direction), 0.0);
	
	diffuseColor = vec4(0);
	specularColor = vec4(0);
	
	if (diffuseFactor > 0) {
		diffuseColor = vec4(col.xyz, 1.0) * col.w * diffuseFactor;
	}
	
	vec3 dirToEye = normalize(eyePos - worldPos);
	vec3 reflectionDirection = normalize(reflect(direction, normal));
	
	float specularFactor = dot(dirToEye, reflectionDirection);
	
	if (specularFactor > 0) {
		specularFactor = pow(specularFactor, specularPower);
		specularColor = vec4(col.xyz, 0.0) * specularIntensity * specularFactor * col.w;
	}
}

vec3 getWorldPos(vec2 UV){
    float z = texture(uTexDepth, UV).r * 2.0 - 1.0;
    float x = UV.x * 2 - 1;
    float y = UV.y * 2 - 1;
    vec4 vProjectedPos = vec4(x, y, z, 1.0f);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
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

float linstep(float low, float high, float v){
	return clamp((v - low) / (high - low), 0.0, 1.0);
}

//returns how birght the pixel is. Ranges from 0 - 1.
float sampleDepth(sampler2D tex, vec2 lUV, float compare){
	vec2 moments =  texture2D(tex, lUV).rg;
	
	float p = step(compare, moments.x);
	float variance = max(moments.y - moments.x * moments.x, 0.00002);
	
	float d = compare - moments.x;
	float pMax = linstep(0.2, 1.0, variance / (variance + d * d));
	
	return min(max(p, pMax), 1.0);
}

void main(void) {
	if (texture(uTexNormal, oUV).a == 0)
		discard;
	
	vec3 normal = (texture(uTexNormal, oUV).xyz - .5) * 2;
	vec3 worldPos = getWorldPos(oUV);
	vec4 lCalcPos = lvpMat * vec4(worldPos, 1.0);
	vec2 lUV = lCalcPos.xy / 2 + .5;
	
	float currentZ = lCalcPos.z * 0.5 + 0.5;
	float visibility = 1;
	
	float v = 1.0 / 9;
	
	visibility -= (1 - sampleDepth(uTexShadowDepth, lUV + vec2(-1, -1) / lSize, currentZ)) * v;
	visibility -= (1 - sampleDepth(uTexShadowDepth, lUV + vec2(-1, 0) / lSize, currentZ)) * v;
	visibility -= (1 - sampleDepth(uTexShadowDepth, lUV + vec2(-1, 1) / lSize, currentZ)) * v;

	visibility -= (1 - sampleDepth(uTexShadowDepth, lUV + vec2(0, -1) / lSize, currentZ)) * v;
	visibility -= (1 - sampleDepth(uTexShadowDepth, lUV + vec2(0, 0) / lSize, currentZ)) * v;
	visibility -= (1 - sampleDepth(uTexShadowDepth, lUV + vec2(0, 1) / lSize, currentZ)) * v;

	visibility -= (1 - sampleDepth(uTexShadowDepth, lUV + vec2(1, -1) / lSize, currentZ)) * v;
	visibility -= (1 - sampleDepth(uTexShadowDepth, lUV + vec2(1, 0) / lSize, currentZ)) * v;
	visibility -= (1 - sampleDepth(uTexShadowDepth, lUV + vec2(1, 1) / lSize, currentZ)) * v;
	
	//makes a smooth transition from shadowmapped area to non-shadowmapped area
	float s = (-sign(currentZ - 1) + 1) / 2;
	visibility = (visibility - 1) * s + 1;
	visibility += smoothstep(lArea - lArea * .2, lArea, length(cross(worldPos - eyePos, lDir)));
	visibility = min(1, visibility);
	
	vec4 diffuse, specular;
	calcLight(diffuse, specular, lCol, lDir, normal, worldPos, texture(uTexLightingData, oUV).r, texture(uTexLightingData, oUV).g);
	
	outColor = diffuse * texture(uTexDiffuse, oUV) * visibility + specular * visibility;
}";

		public const string f_lDirectionalW = @"
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

const float speInt = 1;
const float spePow = 2;

const int numPasses = 4;
const float passValue = 1.0 / numPasses;
const vec2[16] spots = vec2[]
(
	vec2(0.727524718608486,0.519534648172341),
	vec2(0.62559219851419,0.704921784207654),
	vec2(0.775401694129874,0.800459324754988),
	vec2(0.733224332674045,0.819743438539907),
	vec2(0.836471108177896,0.0758824046123225),
	vec2(0.379338752189343,0.120878391024134),
	vec2(0.207462891101587,0.348192278923556),
	vec2(0.64775348065782,0.980569523750138),
	vec2(0.587260726181446,0.435976570209477),
	vec2(0.791940187007161,0.385833041922112),
	vec2(0.121006552186332,0.618705850382664),
	vec2(0.895980558309695,0.685025419893221),
	vec2(0.0554778250192654,0.469948095488338),
	vec2(0.732094154102772,0.50065464037501),
	vec2(0.256260884113731,0.652373947041283),
	vec2(0.344088396217715,0.914511332714237)
);

void calcLight(out vec4 specularColor, vec4 col, vec3 direction, vec3 normal, vec3 worldPos, float specularIntensity, float specularPower) {
	specularColor = vec4(0);
	
	vec3 dirToEye = normalize(eyePos - worldPos);
	vec3 reflectionDirection = normalize(reflect(direction, normal));
	
	float specularFactor = dot(dirToEye, reflectionDirection);
	
	if (specularFactor > 0) {
		specularFactor = pow(specularFactor, specularPower);
		specularColor = col * specularIntensity * specularFactor, specularFactor;
	}
}

vec3 getWorldPos(vec2 UV){
    float z = texture(uTexDepth, UV).r * 2.0 - 1.0;
    float x = UV.x * 2 - 1;
    float y = UV.y * 2 - 1;
    vec4 vProjectedPos = vec4(x, y, z, 1.0f);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void main(void) {
	float lighting = texture(uTexNormal, oUV).a;
	if (lighting == 0)
		discard;
	
	vec3 worldPos = getWorldPos(oUV);
	vec4 lCalcPos = lvpMat * vec4(worldPos, 1.0);
	vec2 lUV = lCalcPos.xy / 2 + .5;
	
	float currentZ = lCalcPos.z * 0.5 + 0.5;
	
	vec3 normal = (texture(uTexNormal, oUV).xyz - .5) * 2;
	float d = dot(normal, lDir);
	float bias = min(0.0025 * (1.0 - d * d), 0.00125);
	float visibility = 1;
	for (int i = 0; i < numPasses; i++){
		vec2 lUVp = lUV;
		visibility -= max(sign(currentZ - ( texture(uTexShadowDepth, lUVp
		//lUV + (vec2(
		//rand(vec2(worldPos.x + spots[i].x, worldPos.y + spots[i].y )) + 
		//rand(vec2(worldPos.x + spots[i].x, worldPos.z + spots[i].y)), 
		//rand(vec2(worldPos.y + spots[i].x, worldPos.z + spots[i].y)) +
		//rand(vec2(worldPos.x + spots[i].x, worldPos.z + spots[i].y))) + spots[i]) / (lSize / 2) 
		).r + bias )), 0) * passValue;
	}
	
	float s = (-sign(currentZ - 1) + 1) / 2;
	visibility = (visibility - 1) * s + 1;
	visibility += smoothstep(lArea - lArea * .2, lArea, length(cross(worldPos - eyePos, lDir)));
	visibility = min(1, visibility);
	
	vec4 specular;
	calcLight(specular, lCol, lDir, normal, worldPos, speInt, spePow);
	
	outColor = specular * visibility;
	outColor *= lighting;
}";
		#endregion
		#region point
		public const string f_lPoint = @"
#version 430

layout(location = 0) out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform samplerCube uTexShadowDepth;
uniform sampler2D uTexLightingData;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec2 lSize;
uniform vec3 lAtt;
uniform float lRange;

uniform vec3 eyePos;
uniform mat4 ipvMat;

uniform vec2 viewPort;

const int numPasses = 4;
const float passValue = 1.0 / numPasses;
const vec2[16] spots = vec2[]
(
	vec2(0.727524718608486,0.519534648172341),
	vec2(0.62559219851419,0.704921784207654),
	vec2(0.775401694129874,0.800459324754988),
	vec2(0.733224332674045,0.819743438539907),
	vec2(0.836471108177896,0.0758824046123225),
	vec2(0.379338752189343,0.120878391024134),
	vec2(0.207462891101587,0.348192278923556),
	vec2(0.64775348065782,0.980569523750138),
	vec2(0.587260726181446,0.435976570209477),
	vec2(0.791940187007161,0.385833041922112),
	vec2(0.121006552186332,0.618705850382664),
	vec2(0.895980558309695,0.685025419893221),
	vec2(0.0554778250192654,0.469948095488338),
	vec2(0.732094154102772,0.50065464037501),
	vec2(0.256260884113731,0.652373947041283),
	vec2(0.344088396217715,0.914511332714237)
);

void calcLight(out vec4 diffuseColor, out vec4 specularColor, vec4 col, vec3 direction, vec3 normal, vec3 worldPos, float specularIntensity, float specularPower) {
	float diffuseFactor = max(dot(normal, -direction), 0.0);
	
	diffuseColor = vec4(0);
	specularColor = vec4(0);
	
	if (diffuseFactor > 0) {
		diffuseColor = vec4(col.xyz, 1.0) * col.w * diffuseFactor;
	}
	
	vec3 dirToEye = normalize(eyePos - worldPos);
	vec3 reflectionDirection = normalize(reflect(direction, normal));
	
	float specularFactor = dot(dirToEye, reflectionDirection);
	
	if (specularFactor > 0) {
		specularFactor = pow(specularFactor, specularPower);
		specularColor = vec4(col.xyz, 0.0) * specularIntensity * specularFactor * col.w;
	}
}

vec3 getWorldPos(vec2 UV){
    float z = texture(uTexDepth, UV).r * 2.0 - 1.0;
    float x = UV.x * 2 - 1;
    float y = UV.y * 2 - 1;
    vec4 vProjectedPos = vec4(x, y, z, 1.0f);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void main(void) {
	vec2 oUV = gl_FragCoord.xy / viewPort;
	if (texture(uTexNormal, oUV).a == 0)
		discard;
	
	vec3 worldPos = getWorldPos(oUV);
	vec3 lwVec = worldPos - lPos;
	float currentZ = length(lwVec);

	if (currentZ > lRange)
		discard;

	vec3 lDir = lwVec / currentZ;
	
	vec3 normal = (texture(uTexNormal, oUV).xyz - .5) * 2;
	float d = dot(normal, lDir);
	float bias = min(0.0025 * (1.0 - d * d), 0.00125);
	float visibility = 1;
	for (int i = 0; i < numPasses; i++)
		visibility -= max(sign( currentZ - texture( uTexShadowDepth, normalize(lDir + vec3( rand(worldPos.yz + spots[i]), rand(worldPos.xz + spots[i]), rand(worldPos.xy + spots[i]) ) / 96) ).r ), 0) * passValue;//???
	
	vec4 diffuse, specular;
	calcLight(diffuse, specular, lCol, lDir, normal, worldPos, texture(uTexLightingData, oUV).r, texture(uTexLightingData, oUV).g);
	
	float attenuation = 0.0001 +
		lAtt.x + 
		lAtt.y * currentZ + 
		lAtt.z * currentZ * currentZ;

	outColor = (diffuse * texture(uTexDiffuse, oUV) * visibility + specular * visibility) / attenuation;

}";
		public const string f_lPointW = @"
#version 430

layout(location = 0) out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform samplerCube uTexShadowDepth;
uniform sampler2D uTexLightingData;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec2 lSize;
uniform vec3 lAtt;
uniform float lRange;

uniform vec3 eyePos;
uniform mat4 ipvMat;

const float speInt = 1;
const float spePow = 2;

uniform vec2 viewPort;

const int numPasses = 4;
const float passValue = 1.0 / numPasses;
const vec2[16] spots = vec2[]
(
	vec2(0.727524718608486,0.519534648172341),
	vec2(0.62559219851419,0.704921784207654),
	vec2(0.775401694129874,0.800459324754988),
	vec2(0.733224332674045,0.819743438539907),
	vec2(0.836471108177896,0.0758824046123225),
	vec2(0.379338752189343,0.120878391024134),
	vec2(0.207462891101587,0.348192278923556),
	vec2(0.64775348065782,0.980569523750138),
	vec2(0.587260726181446,0.435976570209477),
	vec2(0.791940187007161,0.385833041922112),
	vec2(0.121006552186332,0.618705850382664),
	vec2(0.895980558309695,0.685025419893221),
	vec2(0.0554778250192654,0.469948095488338),
	vec2(0.732094154102772,0.50065464037501),
	vec2(0.256260884113731,0.652373947041283),
	vec2(0.344088396217715,0.914511332714237)
);

void calcLight(out vec4 specularColor, vec4 col, vec3 direction, vec3 normal, vec3 worldPos, float specularIntensity, float specularPower) {
	specularColor = vec4(0);
	
	vec3 dirToEye = normalize(eyePos - worldPos);
	vec3 reflectionDirection = normalize(reflect(direction, normal));
	
	float specularFactor = dot(dirToEye, reflectionDirection);
	
	if (specularFactor > 0) {
		specularFactor = pow(specularFactor, specularPower);
		specularColor = vec4(col.xyz, 0.0) * specularIntensity * specularFactor * col.w;
	}
}

vec3 getWorldPos(vec2 UV){
    float z = texture(uTexDepth, UV).r * 2.0 - 1.0;
    float x = UV.x * 2 - 1;
    float y = UV.y * 2 - 1;
    vec4 vProjectedPos = vec4(x, y, z, 1.0f);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void main(void) {
	vec2 oUV = gl_FragCoord.xy / viewPort;
	float lighting = texture(uTexNormal, oUV).a;
	if (lighting == 0)
		discard;
	
	vec3 worldPos = getWorldPos(oUV);
	vec3 lwVec = worldPos - lPos;
	float currentZ = length(lwVec);

	if (currentZ > lRange)
		discard;

	vec3 lDir = lwVec / currentZ;
	
	vec3 normal = (texture(uTexNormal, oUV).xyz - .5) * 2;
	float d = dot(normal, lDir);
	float bias = min(0.0025 * (1.0 - d * d), 0.00125);
	float visibility = 1;
	for (int i = 0; i < numPasses; i++)
		visibility -= max(sign( currentZ - texture( uTexShadowDepth, normalize(lDir + vec3( rand(worldPos.yz + spots[i]), rand(worldPos.xz + spots[i]), rand(worldPos.xy + spots[i]) ) / 96) ).r ), 0) * passValue;//???
	
	vec4 specular;
	calcLight(specular, lCol, lDir, normal, worldPos, speInt, spePow);
	
	float attenuation = 0.0001 +
		lAtt.x + 
		lAtt.y * currentZ + 
		lAtt.z * currentZ * currentZ;

	outColor = specular * visibility / attenuation;
	outColor *= lighting;

}";
		public const string f_lPointNS = @"
#version 430

layout(location = 0) out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform sampler2D uTexLightingData;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec2 lSize;
uniform vec3 lAtt;
uniform float lRange;

uniform vec3 eyePos;
uniform mat4 ipvMat;

uniform vec2 viewPort;

void calcLight(out vec4 diffuseColor, out vec4 specularColor, vec4 col, vec3 direction, vec3 normal, vec3 worldPos, float specularIntensity, float specularPower) {
	float diffuseFactor = max(dot(normal, -direction), 0.0);
	
	diffuseColor = vec4(0);
	specularColor = vec4(0);
	
	if (diffuseFactor > 0) {
		diffuseColor = vec4(col.xyz, 1.0) * col.w * diffuseFactor;
	}
	
	vec3 dirToEye = normalize(eyePos - worldPos);
	vec3 reflectionDirection = normalize(reflect(direction, normal));
	
	float specularFactor = dot(dirToEye, reflectionDirection);
	
	if (specularFactor > 0) {
		specularFactor = pow(specularFactor, specularPower);
		specularColor = vec4(col.xyz, 0.0) * specularIntensity * specularFactor * col.w;
	}
}

vec3 getWorldPos(vec2 UV){
    float z = texture(uTexDepth, UV).r * 2.0 - 1.0;
    float x = UV.x * 2 - 1;
    float y = UV.y * 2 - 1;
    vec4 vProjectedPos = vec4(x, y, z, 1.0f);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void main(void) {
	vec2 oUV = gl_FragCoord.xy / viewPort;
	if (texture(uTexNormal, oUV).a == 0)
		discard;
	
	vec3 worldPos = getWorldPos(oUV);
	vec3 lwVec = worldPos - lPos;
	float currentZ = length(lwVec);

	if (currentZ > lRange)
		discard;

	vec3 lDir = lwVec / currentZ;
	
	vec3 normal = (texture(uTexNormal, oUV).xyz - .5) * 2;
	
	vec4 diffuse, specular;
	calcLight(diffuse, specular, lCol, lDir, normal, worldPos, texture(uTexLightingData, oUV).r, texture(uTexLightingData, oUV).g);
	
	float attenuation = 0.0001 +
		lAtt.x + 
		lAtt.y * currentZ + 
		lAtt.z * currentZ * currentZ;

	outColor = (diffuse * texture(uTexDiffuse, oUV) + specular) / attenuation;

}";
		public const string f_lPointWNS = @"
#version 430

layout(location = 0) out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform sampler2D uTexLightingData;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec2 lSize;
uniform vec3 lAtt;
uniform float lRange;

uniform vec3 eyePos;
uniform mat4 ipvMat;

const float speInt = 1;
const float spePow = 2;

uniform vec2 viewPort;

void calcLight(out vec4 specularColor, vec4 col, vec3 direction, vec3 normal, vec3 worldPos, float specularIntensity, float specularPower) {
	specularColor = vec4(0);
	
	vec3 dirToEye = normalize(eyePos - worldPos);
	vec3 reflectionDirection = normalize(reflect(direction, normal));
	
	float specularFactor = dot(dirToEye, reflectionDirection);
	
	if (specularFactor > 0) {
		specularFactor = pow(specularFactor, specularPower);
		specularColor = vec4(col.xyz, 0.0) * specularIntensity * specularFactor * col.w;
	}
}

vec3 getWorldPos(vec2 UV){
    float z = texture(uTexDepth, UV).r * 2.0 - 1.0;
    float x = UV.x * 2 - 1;
    float y = UV.y * 2 - 1;
    vec4 vProjectedPos = vec4(x, y, z, 1.0f);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void main(void) {
	vec2 oUV = gl_FragCoord.xy / viewPort;
	float lighting = texture(uTexNormal, oUV).a;
	if (lighting == 0)
		discard;
	
	vec3 worldPos = getWorldPos(oUV);
	vec3 lwVec = worldPos - lPos;
	float currentZ = length(lwVec);

	if (currentZ > lRange)
		discard;

	vec3 lDir = lwVec / currentZ;
	
	vec3 normal = (texture(uTexNormal, oUV).xyz - .5) * 2;
	
	vec4 specular;
	calcLight(specular, lCol, lDir, normal, worldPos, speInt, spePow);
	
	float attenuation = 0.0001 +
		lAtt.x + 
		lAtt.y * currentZ + 
		lAtt.z * currentZ * currentZ;

	outColor = specular / attenuation;
	outColor *= lighting;

}";
		#endregion
		#region spot
		public const string f_lSpot = @"
#version 430

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform samplerCube uTexShadowDepth;
uniform sampler2D uTexLightingData;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec3 lAtt;
uniform float lRange;
uniform vec3 lDir;
uniform float lCut;
uniform vec2 lSize;

uniform float zNear;
uniform float zFar;

uniform vec3 eyePos;
uniform mat4 ipvMat;

uniform vec2 viewPort;

const int numPasses = 4;
const float passValue = 1.0 / numPasses;
const vec2[16] spots = vec2[]
(
	vec2(0.727524718608486,0.519534648172341),
	vec2(0.62559219851419,0.704921784207654),
	vec2(0.775401694129874,0.800459324754988),
	vec2(0.733224332674045,0.819743438539907),
	vec2(0.836471108177896,0.0758824046123225),
	vec2(0.379338752189343,0.120878391024134),
	vec2(0.207462891101587,0.348192278923556),
	vec2(0.64775348065782,0.980569523750138),
	vec2(0.587260726181446,0.435976570209477),
	vec2(0.791940187007161,0.385833041922112),
	vec2(0.121006552186332,0.618705850382664),
	vec2(0.895980558309695,0.685025419893221),
	vec2(0.0554778250192654,0.469948095488338),
	vec2(0.732094154102772,0.50065464037501),
	vec2(0.256260884113731,0.652373947041283),
	vec2(0.344088396217715,0.914511332714237)
);

void calcLight(out vec4 diffuseColor, out vec4 specularColor, vec4 col, vec3 direction, vec3 normal, vec3 worldPos, float specularIntensity, float specularPower) {
	float diffuseFactor = max(dot(normal, -direction), 0.0);
	
	diffuseColor = vec4(0);
	specularColor = vec4(0);
	
	if (diffuseFactor > 0) {
		diffuseColor = vec4(col.xyz, 1.0) * col.w * diffuseFactor;
	}
	
	vec3 dirToEye = normalize(eyePos - worldPos);
	vec3 reflectionDirection = normalize(reflect(direction, normal));
	
	float specularFactor = dot(dirToEye, reflectionDirection);
	
	if (specularFactor > 0) {
		specularFactor = pow(specularFactor, specularPower);
		specularColor = vec4(col.xyz, 0.0) * specularIntensity * specularFactor * col.w;
	}
}

vec3 getWorldPos(vec2 UV){
    float z = texture(uTexDepth, UV).r * 2.0f - 1.0f;
    float x = UV.x * 2 - 1;
    float y = UV.y * 2 - 1;
    vec4 vProjectedPos = vec4(x, y, z, 1.0f);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
}

vec4 calcPointLight(vec2 oUV, vec3 worldPos, vec3 lightDir, vec3 normal, float distanceToPoint){
	if (distanceToPoint > lRange)
		discard;
	
	vec4 diffuse, specular;
	calcLight(diffuse, specular, lCol, lightDir, normal, worldPos, texture(uTexLightingData, oUV).r, texture(uTexLightingData, oUV).g);
	
	float attenuation = 0.0001 +
		lAtt.x + 
		lAtt.y * distanceToPoint + 
		lAtt.z * distanceToPoint * distanceToPoint;
	
	return (diffuse + specular) / attenuation;
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void main(void) {
	
	vec2 oUV = gl_FragCoord.xy / viewPort;
	if (texture(uTexNormal, oUV).a == 0)
		discard;

	vec3 worldPos = getWorldPos(oUV);
	vec3 lightDir = worldPos - lPos;
	float distanceToPoint = length(lightDir);

	if (distanceToPoint > lRange)
		discard;

	lightDir = normalize(lightDir);
	float spotFactor = dot(lightDir, lDir);
	
	outColor = vec4(0);
	
	if (spotFactor > lCut) {
		vec3 normal = (texture(uTexNormal, oUV).xyz - .5) * 2;
		float d = dot(normal, lightDir);
		float bias = min(0.0025 * (1.0 - d * d), 0.00125);
		float visibility = 1;
		for (int i = 0; i < numPasses; i++)
			visibility -= max(sign( distanceToPoint - texture( uTexShadowDepth, normalize(lightDir + vec3( rand(worldPos.yz + spots[i]), rand(worldPos.xz + spots[i]), rand(worldPos.xy + spots[i]) ) / 96) ).r ), 0) * passValue;//???
		
		outColor = calcPointLight(oUV, worldPos, lightDir, normal, distanceToPoint) * 
		(1 - (1 - spotFactor) / (1 - lCut)) * visibility;
	}

}";
		public const string f_lSpotW = @"
#version 430

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform samplerCube uTexShadowDepth;
uniform sampler2D uTexLightingData;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec3 lAtt;
uniform float lRange;
uniform vec3 lDir;
uniform float lCut;
uniform vec2 lSize;

uniform float zNear;
uniform float zFar;

uniform vec3 eyePos;
uniform mat4 ipvMat;

const float speInt = 1;
const float spePow = 2;

uniform vec2 viewPort;

const int numPasses = 4;
const float passValue = 1.0 / numPasses;
const vec2[16] spots = vec2[]
(
	vec2(0.727524718608486,0.519534648172341),
	vec2(0.62559219851419,0.704921784207654),
	vec2(0.775401694129874,0.800459324754988),
	vec2(0.733224332674045,0.819743438539907),
	vec2(0.836471108177896,0.0758824046123225),
	vec2(0.379338752189343,0.120878391024134),
	vec2(0.207462891101587,0.348192278923556),
	vec2(0.64775348065782,0.980569523750138),
	vec2(0.587260726181446,0.435976570209477),
	vec2(0.791940187007161,0.385833041922112),
	vec2(0.121006552186332,0.618705850382664),
	vec2(0.895980558309695,0.685025419893221),
	vec2(0.0554778250192654,0.469948095488338),
	vec2(0.732094154102772,0.50065464037501),
	vec2(0.256260884113731,0.652373947041283),
	vec2(0.344088396217715,0.914511332714237)
);

void calcLight(out vec4 specularColor, vec4 col, vec3 direction, vec3 normal, vec3 worldPos, float specularIntensity, float specularPower) {
	specularColor = vec4(0);
	
	vec3 dirToEye = normalize(eyePos - worldPos);
	vec3 reflectionDirection = normalize(reflect(direction, normal));
	
	float specularFactor = dot(dirToEye, reflectionDirection);
	
	if (specularFactor > 0) {
		specularFactor = pow(specularFactor, specularPower);
		specularColor = vec4(col.xyz, 0.0) * specularIntensity * specularFactor * col.w;
	}
}

vec3 getWorldPos(vec2 UV){
    float z = texture(uTexDepth, UV).r * 2.0f - 1.0f;
    float x = UV.x * 2 - 1;
    float y = UV.y * 2 - 1;
    vec4 vProjectedPos = vec4(x, y, z, 1.0f);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
}

vec4 calcPointLight(vec2 oUV, vec3 worldPos, vec3 lightDir, vec3 normal, float distanceToPoint){
	if (distanceToPoint > lRange)
		discard;
	
	vec4 specular;
	calcLight(specular, lCol, lightDir, normal, worldPos, speInt, spePow);
	
	float attenuation = 0.0001 +
		lAtt.x + 
		lAtt.y * distanceToPoint + 
		lAtt.z * distanceToPoint * distanceToPoint;
	
	return specular / attenuation;
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void main(void) {
	
	vec2 oUV = gl_FragCoord.xy / viewPort;
	float lighting = texture(uTexNormal, oUV).a;
	if (lighting == 0)
		discard;

	vec3 worldPos = getWorldPos(oUV);
	vec3 lightDir = worldPos - lPos;
	float distanceToPoint = length(lightDir);

	if (distanceToPoint > lRange)
		discard;

	lightDir = normalize(lightDir);
	float spotFactor = dot(lightDir, lDir);
	
	outColor = vec4(0);
	
	if (spotFactor > lCut) {
		vec3 normal = (texture(uTexNormal, oUV).xyz - .5) * 2;
		float d = dot(normal, lightDir);
		float bias = min(0.0025 * (1.0 - d * d), 0.00125);
		float visibility = 1;
		for (int i = 0; i < numPasses; i++)
			visibility -= max(sign( distanceToPoint - texture( uTexShadowDepth, normalize(lightDir + vec3( rand(worldPos.yz + spots[i]), rand(worldPos.xz + spots[i]), rand(worldPos.xy + spots[i]) ) / 96) ).r ), 0) * passValue;//???
		
		outColor = calcPointLight(oUV, worldPos, lightDir, normal, distanceToPoint) * 
		(1 - (1 - spotFactor) / (1 - lCut)) * visibility;
		outColor *= lighting;
	}

}";
		public const string f_lSpotNS = @"
#version 430

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform sampler2D uTexLightingData;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec3 lAtt;
uniform float lRange;
uniform vec3 lDir;
uniform float lCut;

uniform vec3 eyePos;
uniform mat4 ipvMat;

uniform vec2 viewPort;

void calcLight(out vec4 diffuseColor, out vec4 specularColor, vec4 col, vec3 direction, vec3 normal, vec3 worldPos, float specularIntensity, float specularPower) {
	float diffuseFactor = max(dot(normal, -direction), 0.0);
	
	diffuseColor = vec4(0);
	specularColor = vec4(0);
	
	if (diffuseFactor > 0) {
		diffuseColor = vec4(col.xyz, 1.0) * col.w * diffuseFactor;
	}
	
	vec3 dirToEye = normalize(eyePos - worldPos);
	vec3 reflectionDirection = normalize(reflect(direction, normal));
	
	float specularFactor = dot(dirToEye, reflectionDirection);
	
	if (specularFactor > 0) {
		specularFactor = pow(specularFactor, specularPower);
		specularColor = vec4(col.xyz, 0.0) * specularIntensity * specularFactor * col.w;
	}
}

vec3 getWorldPos(vec2 UV){
    float z = texture(uTexDepth, UV).r * 2.0f - 1.0f;
    float x = UV.x * 2 - 1;
    float y = UV.y * 2 - 1;
    vec4 vProjectedPos = vec4(x, y, z, 1.0f);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
}

vec4 calcPointLight(vec2 oUV, vec3 worldPos, vec3 lightDir, float distanceToPoint){
	if (distanceToPoint > lRange)
		discard;
	
	vec4 diffuse, specular;
	calcLight(diffuse, specular, lCol, lightDir, (texture(uTexNormal, oUV).xyz - .5) * 2, worldPos, texture(uTexLightingData, oUV).r, texture(uTexLightingData, oUV).g);
	
	float attenuation = 0.0001 +
		lAtt.x + 
		lAtt.y * distanceToPoint + 
		lAtt.z * distanceToPoint * distanceToPoint;
	
	return (diffuse + specular) / attenuation;
}

void main(void) {
	
	vec2 oUV = gl_FragCoord.xy / viewPort;
	if (texture(uTexNormal, oUV).a == 0)
		discard;

	vec3 worldPos = getWorldPos(oUV);
	vec3 lightDir = worldPos - lPos;
	float distanceToPoint = length(lightDir);

	if (distanceToPoint > lRange)
		discard;

	lightDir = normalize(lightDir);
	
	float spotFactor = dot(lightDir, lDir);
	
	outColor = vec4(0);
	
	if (spotFactor > lCut) {
		outColor = calcPointLight(oUV, worldPos, lightDir, distanceToPoint) * 
		(1 - (1 - spotFactor) / (1 - lCut));
	}
}";
		public const string f_lSpotWNS = @"
#version 430

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexDepth;
uniform sampler2D uTexLightingData;
uniform vec4 lCol;
uniform vec3 lPos;
uniform vec3 lAtt;
uniform float lRange;
uniform vec3 lDir;
uniform float lCut;

uniform vec3 eyePos;
uniform mat4 ipvMat;

const float speInt = 1;
const float spePow = 2;

uniform vec2 viewPort;

void calcLight(out vec4 specularColor, vec4 col, vec3 direction, vec3 normal, vec3 worldPos, float specularIntensity, float specularPower) {
	specularColor = vec4(0);
	
	vec3 dirToEye = normalize(eyePos - worldPos);
	vec3 reflectionDirection = normalize(reflect(direction, normal));
	
	float specularFactor = dot(dirToEye, reflectionDirection);
	
	if (specularFactor > 0) {
		specularFactor = pow(specularFactor, specularPower);
		specularColor = vec4(col.xyz, 0.0) * specularIntensity * specularFactor * col.w;
	}
}

vec3 getWorldPos(vec2 UV){
    float z = texture(uTexDepth, UV).r * 2.0f - 1.0f;
    float x = UV.x * 2 - 1;
    float y = UV.y * 2 - 1;
    vec4 vProjectedPos = vec4(x, y, z, 1.0f);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
}

vec4 calcPointLight(vec2 oUV, vec3 worldPos, vec3 lightDir, float distanceToPoint){
	if (distanceToPoint > lRange)
		discard;
	
	vec4 specular;
	calcLight(specular, lCol, lightDir, (texture(uTexNormal, oUV).xyz - .5) * 2, worldPos, speInt, spePow);
	
	float attenuation = 0.0001 +
		lAtt.x + 
		lAtt.y * distanceToPoint + 
		lAtt.z * distanceToPoint * distanceToPoint;
	
	return specular / attenuation;
}

void main(void) {
	
	vec2 oUV = gl_FragCoord.xy / viewPort;
	float lighting = texture(uTexNormal, oUV).a;
	if (lighting == 0)
		discard;

	vec3 worldPos = getWorldPos(oUV);
	vec3 lightDir = worldPos - lPos;
	float distanceToPoint = length(lightDir);

	if (distanceToPoint > lRange)
		discard;

	lightDir = normalize(lightDir);
	
	float spotFactor = dot(lightDir, lDir);
	
	outColor = vec4(0);
	
	if (spotFactor > lCut) {
		outColor = calcPointLight(oUV, worldPos, lightDir, distanceToPoint) * 
		(1 - (1 - spotFactor) / (1 - lCut));
		outColor *= lighting;
	}
}";
		#endregion
		#endregion

		#region surface2
		public const string v_surface2 = @"
#version 430

in vec2 vPos;

out vec2 oUV;

void main(void){
	oUV = vPos / 2 + .5f;
	gl_Position = vec4(vPos, 0, 1);
}";
		public const string f_surface2 = @"
#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexture;

void main(void) {
	
	outColor = texture(uTexture, oUV);
	
}";
		#endregion

		#region surface3
		public const string v_surface3 = @"
#version 430

in vec3 vPos;

uniform mat4 uMVP_mat;

void main(void){
	gl_Position = uMVP_mat * vec4(vPos, 1);
}";
		#endregion

		#region text
		public const string v_text = @"
#version 430

in vec3 vPos;
in vec3 vCol;
in vec2 vUV;

out vec4 oCol;
out vec4 oPos;
out vec2 oUV;

uniform mat4 uMVP_mat;

void main(void){
	oCol = vec4(vCol, 1);
	oPos = vec4(vPos, 1);
	oUV = vUV;
	gl_Position = uMVP_mat * oPos;
}";
		public const string f_text = @"
#version 430

in vec4 oCol;
in vec2 oUV;

out vec4 outColor;

uniform sampler2D uDiffuse;
uniform vec4 uColor;

void main(void) {
	
	float d = 1 - texture(uDiffuse, oUV).a;
	
	float mixRatio = smoothstep(.5, .8, d);
	float alpha = 1 - smoothstep(.55, .85, d);
	
	outColor = vec4(oCol.rgb * mix(uColor.xyz, 1 - uColor.xyz, mixRatio), alpha * uColor.a);
}";
		#endregion

		#region text instanced
		public const string v_textI = @"
#version 430

in vec2 vPos;
in vec4 iTra;
in vec4 iUV;
in vec4 iCol;

out vec2 oUV;
out vec4 oCol;

uniform mat4 uMVP_mat;

void main(void){
	oUV = vPos * iUV.zw + iUV.xy;
	gl_Position = uMVP_mat * vec4(vPos * iTra.zw + iTra.xy, 0, 1);
	oCol = iCol;
}";
		public const string f_textI = @"
#version 430

in vec2 oUV;
in vec4 oCol;

out vec4 outColor;

uniform sampler2D uDiffuse;
uniform vec4 uColor;

void main(void) {
	
	float d = 1 - texture(uDiffuse, oUV).a;
	
	float mixRatio = smoothstep(.475, .625, d);
	float alpha = 1 - smoothstep(.575, .65, d);
	
	outColor = vec4(0);
	outColor += vec4( mix(uColor.xyz * oCol.rgb, 1 - uColor.xyz * oCol.rgb, mixRatio), alpha);
	outColor.a *= uColor.a * oCol.a;
}";
		#endregion
	}

}
