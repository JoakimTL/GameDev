#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexture;

void main(void) {
	outColor = vec4(1, 0.3, 0.7, 0);//texture(uTexture, oUV);
}