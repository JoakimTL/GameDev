#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec4 Position;
	vec2 UV;
	vec4 Color;
	bool Fill;
	bool Flip;
} IN;

layout(location = 0) out vec4 outColor;

void main(void) {
	bool fillCurve = IN.UV.y < IN.UV.x * IN.UV.x;
	bool fillPixel = IN.Fill || (fillCurve && !IN.Flip) || (!fillCurve && IN.Flip);
	if (!fillPixel) 
		discard;
	outColor = IN.Color;
}