#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform vec2 tSize;
uniform float uBlur;
uniform vec2 uBlurCenter;

void main(void) {
	
	vec2 dir = (oUV * 2 - 1) - (uBlurCenter);
	vec2 rSize = dir * uBlur / tSize;
	vec4 blur = vec4(0);
	
	for (float i = -7; i <= 7; i++)
		blur += texture(uTexDiffuse, oUV + vec2(i) * rSize);
	
	outColor = blur / 15;
}