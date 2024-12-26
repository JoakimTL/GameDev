#version 430

in vec2 oUV;

out float outColor;

uniform sampler2D uTexture;

void main(void) {
	outColor = texture(uTexture, oUV).r;
}