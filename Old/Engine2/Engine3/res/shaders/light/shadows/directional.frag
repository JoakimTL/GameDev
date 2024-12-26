#version 430

in vec2 oUV;

layout(location = 0) out vec2 depthData;
layout(location = 1) out vec4 diffuseData;

uniform sampler2D uTexDiffuse;

void main(void) {
	if (texture(uTexDiffuse, oUV).a < 0.5)
		discard;
	float depth = gl_FragCoord.z;
	float dx = dFdx(depth);
	float dy = dFdy(depth);
	float momentZ = depth * depth + 0.25 * (dx * dx + dy * dy);
	depthData = vec2(depth, momentZ);
	diffuseData = vec4(1, 1, 1, 1);
}