#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D gloTex;
uniform vec3 uCol;

void main(void) {
	outColor = vec4((texture(uTexDiffuse, oUV).rgb) * texture(uTexNormal, oUV).a * (vec3(1) - uCol * texture(uTexDiffuse, oUV).a), 1) + texture(gloTex, oUV);
}