#version 430

layout(location = 0) out vec2 depthData;

void main(void) {
	float depth = gl_FragCoord.z;
	float dx = dFdx(depth);
	float dy = dFdy(depth);
	float momentZ = depth * depth + 0.25 * (dx * dx + dy * dy);
	depthData = vec2(depth, momentZ);
}