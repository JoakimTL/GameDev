#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexLightingData;
uniform sampler2D uTexGlow;
uniform sampler2D uParticleDiffuse;
uniform sampler2D uParticleGlow;
uniform vec3 uCol;

void main(void) {
	vec4 diff = texture(uTexDiffuse, oUV);
	vec4 glow = texture(uTexGlow, oUV);
	outColor = vec4(
		  diff.rgb * uCol * diff.a * texture(uTexLightingData, oUV).b
		+ glow.rgb * glow.a
		, 1
	);
}