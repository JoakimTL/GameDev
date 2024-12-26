#version 430

in vec2 oUV;
in vec2 oUVStart;

layout(location = 0) out vec2 depthData;
layout(location = 1) out vec4 diffuseData;

uniform sampler2D uTexDiffuse;

void main(void) {
	vec2 uv = oUVStart + mod(oUV, 1.0)/256;
	uv.y = 1 - uv.y;
	if (texture(uTexDiffuse, uv).a < 0.5)
		discard;
	float depth = gl_FragCoord.z;
	float dx = dFdx(depth);
	float dy = dFdy(depth);
	float momentZ = depth * depth + 0.25 * (dx * dx + dy * dy);
	depthData = vec2(depth, momentZ);
	diffuseData = vec4(1, 1, 1, 1);
}