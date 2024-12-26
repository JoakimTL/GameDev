#version 430

in vec4 oCol;
in vec4 oPos;
in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexture;
uniform sampler2D uStencil;
uniform vec4 uColor;

float when_eq(float x, float y) {
  return 1.0 - abs(sign(x - y));
}

void main(void) {
	outColor = texture(uTexture, oUV) * oCol * uColor * when_eq(texture(uStencil, (oPos.xy + 1) * 0.5).r, 0);
}