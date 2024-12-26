#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform vec2 tSize;
uniform vec2 uBlur;

void main(void) {
	
	vec2 rSize = uBlur / tSize;
	vec4 gauss = vec4(0);
	
	gauss += texture(uTexDiffuse, oUV + vec2(-3.0) * rSize) * 1.0  / 64;
	gauss += texture(uTexDiffuse, oUV + vec2(-2.0) * rSize) * 6.0  / 64;
	gauss += texture(uTexDiffuse, oUV + vec2(-1.0) * rSize) * 15.0 / 64;
	gauss += texture(uTexDiffuse, oUV + vec2( 0.0) * rSize) * 20.0 / 64;
	gauss += texture(uTexDiffuse, oUV + vec2( 1.0) * rSize) * 15.0 / 64;
	gauss += texture(uTexDiffuse, oUV + vec2( 2.0) * rSize) * 6.0  / 64;
	gauss += texture(uTexDiffuse, oUV + vec2( 3.0) * rSize) * 1.0  / 64;
	
	outColor = gauss;
	
}