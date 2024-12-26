#version 430

in vec3 oPos;

layout(location = 0) out vec2 depthData;

uniform vec3 lightPos;
uniform float lRange;

void main(void) {
    float depth = length(lightPos - oPos);
	float dx = dFdx(depth);
	float dy = dFdy(depth);
	float momentZ = depth * depth + 0.25 * (dx * dx + dy * dy);
	depthData = vec2(depth, momentZ);
}