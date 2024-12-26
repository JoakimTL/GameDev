#version 430

in vec4 oPos;
in vec4 oCol;

uniform vec4 uColor;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec3 outNormal;
layout(location = 2) out vec3 outLightInfo;
layout(location = 3) out vec4 outGlow;
layout(location = 4) out vec4 outParticles;

void main(void) {
	if (oCol.a * uColor.a == 0.0)
		discard;
	outColor = outGlow = oCol * uColor;
}