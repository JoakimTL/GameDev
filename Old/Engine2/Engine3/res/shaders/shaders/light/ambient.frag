#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexLightingData;
uniform sampler2D uTexGlow;
uniform vec3 uCol;

void main(void) {
	vec4 glow = texture(uTexGlow, oUV);
	outColor = vec4(
		texture(uTexDiffuse, oUV).rgb * uCol *
		texture(uTexLightingData, oUV).b * texture(uTexDiffuse, oUV).a
		+ glow.rgb * glow.a
		, 1
	);
}