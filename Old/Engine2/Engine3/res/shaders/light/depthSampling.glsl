float linstep(float low, float high, float v){
	return clamp((v - low) / (high - low), 0.0, 1.0);
}

float sampleDepth(samplerCube tex, vec3 lUV, float compare, float lRange){
	vec2 moments = texture(tex, lUV).rg;
	
	float p = step(compare / lRange, moments.x);
	float variance = max(moments.y - moments.x * moments.x, 0.000001);
	
	float d = compare / lRange - moments.x;
	float pMax = variance / (variance + d * d);
	p *= p;
	pMax *= pMax;
	
	return min(max(p, pMax), 1.0);
}

/*float sampleDepth(sampler2D tex, vec2 lUV, float compare){
	vec2 moments = texture2D(tex, lUV).rg;
	
	float p = step(compare, moments.x);
	float variance = max(moments.y - moments.x * moments.x, 0.000001);
	
	float d = compare - moments.x;
	float pMax = variance / (variance + d * d);
	p *= p;
	pMax *= pMax;
	
	return min(max(p, pMax), 1.0);
}

float sampleDepth(samplerCube tex, vec3 lUV, float compare, float lRange){
	vec2 moments = texture(tex, lUV).rg;

	float p = step(compare, moments.x);
	float variance = max(moments.y - moments.x * moments.x, 0.0015);
	
	float d = compare - moments.x;
	float pMax = linstep(0.2, 1.0, variance / (variance + d * d));
	
	return min(max(p, pMax), 1.0);
}*/

float sampleDepth(sampler2D tex, vec2 lUV, float compare){
	vec2 moments =  texture2D(tex, lUV).rg;
	
	float p = step(compare, moments.x);
	float variance = max(moments.y - moments.x * moments.x, 0.00001);
	
	float d = compare - moments.x;
	float pMax = linstep(0.5, 1.0, variance / (variance + d * d));
	
	return min(max(p, pMax), 1.0);
}