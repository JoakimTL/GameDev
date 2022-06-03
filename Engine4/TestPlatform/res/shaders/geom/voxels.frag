#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec3 normal;
	vec4 diffuseColor;
	vec4 glowColor;
	flat float metallic;
	flat float roughness;
} IN;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec3 outNormal;
layout(location = 2) out vec2 outLightInfo;
layout(location = 3) out vec4 outGlow;

void main(void) {
	outColor = IN.diffuseColor;
	outNormal = IN.normal * 0.5 + 0.5;
	outLightInfo = vec2(IN.metallic, IN.roughness);
	outGlow = IN.glowColor;
}