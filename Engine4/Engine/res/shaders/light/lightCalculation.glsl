float distributionGGX(float NdotH, float roughness){
	float a = roughness * roughness;
	float a2 = a * a;
	float denom = NdotH * NdotH * (a2 - 1.0) + 1.0;
	denom = 3.14159265358979 * denom * denom;
	return a2 / max(denom, 0.0000001);
}

float geometrySmith(float NdotV, float NdotL, float roughness){
	float r = roughness + 1.0;
	float k = (r*r) / 8.0;
	float ggx1 = NdotV / (NdotV * (1.0 - k) + k);
	float ggx2 = NdotL / (NdotL * (1.0 - k) + k);
	return ggx1 * ggx2;
}

vec3 fresnelSchlick(float NdotL, vec3 baseReflectivity){
	return baseReflectivity + (1.0 - baseReflectivity) * pow(1.0 - NdotL, 5.0);
}

vec3 getWorldPos(vec2 UV, float sampledDepth, mat4 ipvMat){
    float z = sampledDepth * 2.0 - 1.0;
    float x = UV.x * 2.0 - 1.0;
    float y = UV.y * 2.0 - 1.0;
    vec4 vProjectedPos = vec4(x, y, z, 1.0);
    vec4 vPositionVS = ipvMat * vProjectedPos;
	return vPositionVS.xyz / vPositionVS.w;
}

vec3 calculateLightPBR(vec3 lightColor,	vec3 ePos, vec3 wPos, vec3 lDir, vec3 normal, vec3 albedo, float metallic, float roughness) {
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

	return lightColor * (kD * albedo / 3.14159265358979 + specular) * NdotL;
}