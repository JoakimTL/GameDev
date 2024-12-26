#include pbr.glsl

void calcLightPBR(out vec3 outColor, vec3 lightColor,
		vec3 ePos, vec3 wPos, vec3 lDir, vec3 normal,
		vec3 albedo, float metallic, float roughness) {
	vec3 V = normalize(ePos - wPos);
	vec3 baseReflectivity = mix(vec3(0.04), albedo, metallic);
	vec3 H = normalize(V + lDir);
	
	float NdotV = max(dot(normal, V), 0.0000001);
	float NdotL = max(dot(normal, lDir), 0.0000001);
	float NdotH = max(dot(normal, H), 0.0);
	float HdotV = max(dot(H, V), 0.0);
	
	float D = distributionGGX(NdotH, roughness);
	float G = geometrySmith(NdotV, NdotL, roughness);
	vec3 F = fresnelSchlick(NdotL, baseReflectivity);

	vec3 specular = D * G * F / ( 4.0 * NdotV * NdotL );

	vec3 kD = vec3(1.0) - specular;
	kD *= 1.0 - metallic;

	outColor = lightColor * (kD * albedo / 3.14159265358979 + specular) * NdotL;
	
}

vec3 getWorldPos(vec2 UV, sampler2D texDepth, mat4 ipvMat){
    float z = texture2D(texDepth, UV).r * 2.0 - 1.0;
    float x = UV.x * 2.0 - 1.0;
    float y = UV.y * 2.0 - 1.0;
    vec4 vProjectedPos = vec4(x, y, z, 1.0);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
}

/*
void calcLightPBR(out vec3 outColor, vec3 lightColor,
		vec3 ePos, vec3 wPos, vec3 lDir, vec3 normal,
		vec3 albedo, float metallic, float roughness) {
	vec3 V = normalize(ePos - wPos);
	vec3 baseReflectivity = mix(vec3(0.04), albedo, metallic);
	vec3 H = normalize(V + lDir);
	
	float NdotV = max(dot(normal, V), 0.0000001);
	float NdotL = max(dot(normal, lDir), 0.0000001);
	float NdotH = max(dot(normal, H), 0.0);
	float HdotV = max(dot(H, V), 0.0);
	
	float D = distributionGGX(NdotH, roughness);
	float G = geometrySmith(NdotV, NdotL, roughness);
	vec3 F = fresnelSchlick(HdotV, baseReflectivity);

	vec3 specular = D * G * F * dot(normalize(reflect(-lDir, normal)), V);

	vec3 kD = (1.0 - specular);

	outColor = lightColor * (kD * albedo / 3.14159265358979 + specular) * NdotL;
}
*/