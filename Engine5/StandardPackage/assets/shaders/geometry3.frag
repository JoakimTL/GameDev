#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec4 Position;
	vec2 UV;
	vec3 Normal;
	vec4 Color;
	float NormalMapped;
	flat uint DiffuseTexture;
	flat uint NormalTexture;
	flat uint LightingTexture;
	flat uint GlowTexture;
} IN;

layout (std430) buffer TextureAddresses
{ 
	uvec2 Textures[65536];
} ta;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec3 outNormal;
layout(location = 2) out vec2 outLightInfo;
layout(location = 3) out vec4 outGlow;

void main(void) {
	uvec2 diffuseTextureHandle = ta.Textures[IN.DiffuseTexture];
	sampler2D diffuseTex = sampler2D(diffuseTextureHandle);
	outColor = texture(diffuseTex, IN.UV) * IN.Color;
	//if (outColor.a < 0.5)
	//	discard;
	
	uvec2 normalTextureHandle = ta.Textures[IN.NormalTexture];
	sampler2D normalTex = sampler2D(normalTextureHandle);
	uvec2 lightingTextureHandle = ta.Textures[IN.LightingTexture];
	sampler2D lightingTex = sampler2D(lightingTextureHandle);
	uvec2 glowTextureHandle = ta.Textures[IN.GlowTexture];
	sampler2D glowTex = sampler2D(glowTextureHandle);

	outNormal = normalize(IN.Normal + (texture(normalTex, IN.UV).rgb * 2.0 - 1.0) * IN.NormalMapped) * 0.5 + 0.5;
	outLightInfo = texture(lightingTex, IN.UV).rg;
	outGlow = texture(glowTex, IN.UV) * IN.Color;
}