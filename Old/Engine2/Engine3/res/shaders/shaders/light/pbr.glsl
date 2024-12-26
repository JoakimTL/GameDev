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
//https://www.youtube.com/watch?v=5p0e7YNONr8&list=PLIbUZ3URbL0ESKHrvzXuHjrcLi7gxhBby&index=29
//https://learnopengl.com/PBR/Theory