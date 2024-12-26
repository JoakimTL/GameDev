#version 430

in vec3 oPos;
in vec2 oUV;

layout(location = 0) out vec2 depthData;

uniform sampler2D uTexDiffuse;
uniform vec3 lightPos;
uniform float lRange;

void main(void) {
	if (texture(uTexDiffuse, oUV).a < 0.5)
		discard;
    float depth = length(lightPos - oPos) / lRange;
	float dx = dFdx(depth);
	float dy = dFdy(depth);
	float momentZ = depth * depth + 0.25 * (dx * dx + dy * dy);
	depthData = vec2(depth, momentZ);
}