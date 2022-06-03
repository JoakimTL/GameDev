#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec4 Position;
	vec2 UV;
	vec4 Color;
	flat uint64_t Texture1;
	flat uint64_t Texture2;
	flat float Blend;
} IN;

layout(location = 4) out vec4 outAccumulation;
layout(location = 5) out float outReveal;

void main(void) {
	
	sampler2D tex1 = sampler2D(IN.Texture1);
	sampler2D tex2 = sampler2D(IN.Texture2);

	vec4 color = (texture(tex1, IN.UV) * (1 - IN.Blend) + texture(tex2, IN.UV) * IN.Blend) * IN.Color;

	float weight = clamp(pow(min(1.0, color.a * 10.0) + 0.01, 3.0) * 1e8 * pow(1.0 - gl_FragCoord.z * 0.9, 3.0), 1e-2, 3e3);

    outAccumulation = vec4(color.rgb * color.a, color.a) * weight;

    outReveal = color.a;
}