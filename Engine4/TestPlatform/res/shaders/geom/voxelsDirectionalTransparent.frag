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

layout(location = 0) out vec4 outAccumulation;
layout(location = 1) out float outReveal;

void main(void) {
	float weight = clamp(pow(min(1.0, IN.diffuseColor.a * 10.0) + 0.01, 3.0) * 1e8 * pow(1.0 - gl_FragCoord.z * 0.9, 3.0), 1e-2, 3e3);
    outAccumulation = vec4(IN.diffuseColor.rgb * IN.diffuseColor.a, IN.diffuseColor.a) * weight;
    outReveal = IN.diffuseColor.a;
}