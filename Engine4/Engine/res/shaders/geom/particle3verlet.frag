#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec4 Position;
	vec2 UV;
	vec4 Color;
} IN;

layout(location = 4) out vec4 outAccumulation;
layout(location = 5) out float outReveal;

void main(void) {
	vec4 color = IN.Color;
	float weight = clamp(pow(min(1.0, color.a * 10.0) + 0.01, 3.0) * 1e8 * pow(1.0 - gl_FragCoord.z * 0.9, 3.0), 1e-2, 3e3);

    outAccumulation = vec4(color.rgb * color.a, color.a) * weight;
    outReveal = color.a;
}