#version 430

in vec4 oCol;
in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexture;
uniform vec4 uColor;

void main(void) {
	
	outColor = texture(uTexture, oUV) * oCol * uColor;
	
}