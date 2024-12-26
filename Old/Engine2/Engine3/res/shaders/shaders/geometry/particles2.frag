#version 430

in vec4 oPos;
in vec4 oTex;
in vec4 oColor;
in float oBlend;

uniform sampler2D uTexture;

layout(location = 0) out vec4 outParticles;

void main(void){
	outParticles = mix(texture(uTexture, oTex.xy), texture(uTexture, oTex.zw), oBlend) * oColor;
}