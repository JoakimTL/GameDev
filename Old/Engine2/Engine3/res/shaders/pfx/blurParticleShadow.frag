#version 430

in vec2 oUV;

layout(location = 0) out vec2 outColor;

uniform sampler2D uTexDiffuse;
uniform vec2 tSize;
uniform vec2 uBlur;

void main(void) {
	
	vec2 rSize = uBlur / tSize;
	vec2 gauss = vec2(0);
	
	gauss += texture(uTexDiffuse, oUV + vec2(-3.0) * rSize).r * 1.0  / 64;
	gauss += texture(uTexDiffuse, oUV + vec2(-2.0) * rSize).r * 6.0  / 64;
	gauss += texture(uTexDiffuse, oUV + vec2(-1.0) * rSize).r * 15.0 / 64;
	gauss += texture(uTexDiffuse, oUV + vec2( 0.0) * rSize).r * 20.0 / 64;
	gauss += texture(uTexDiffuse, oUV + vec2( 1.0) * rSize).r * 15.0 / 64;
	gauss += texture(uTexDiffuse, oUV + vec2( 2.0) * rSize).r * 6.0  / 64;
	gauss += texture(uTexDiffuse, oUV + vec2( 3.0) * rSize).r * 1.0  / 64;
	
	outColor = gauss;
	
}