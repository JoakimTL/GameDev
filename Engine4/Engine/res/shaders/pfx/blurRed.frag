#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PFXVertex {
    vec2 uv;
} IN;

layout (std140) uniform PFXBlock
{ 
	uint64_t textureToBlur;
	vec2 textureSize;
	vec2 blurSize;
} pfx;

layout(location = 0) out float outColor;

void main(void) {
	sampler2D textureSampler = sampler2D(pfx.textureToBlur);
	vec2 rSize = pfx.blurSize / pfx.textureSize;
	float gauss = 0;
	
	gauss += texture(textureSampler, IN.uv + vec2(-3.0) * rSize).r * 1.0  / 64;
	gauss += texture(textureSampler, IN.uv + vec2(-2.0) * rSize).r * 6.0  / 64;
	gauss += texture(textureSampler, IN.uv + vec2(-1.0) * rSize).r * 15.0 / 64;
	gauss += texture(textureSampler, IN.uv + vec2( 0.0) * rSize).r * 20.0 / 64;
	gauss += texture(textureSampler, IN.uv + vec2( 1.0) * rSize).r * 15.0 / 64;
	gauss += texture(textureSampler, IN.uv + vec2( 2.0) * rSize).r * 6.0  / 64;
	gauss += texture(textureSampler, IN.uv + vec2( 3.0) * rSize).r * 1.0  / 64;
	
	outColor = gauss;
}