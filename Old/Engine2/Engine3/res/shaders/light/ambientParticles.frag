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
	vec4 diffp = texture(uParticleDiffuse, oUV);
	vec4 glowP = texture(uParticleGlow, oUV);
	outColor = vec4(
		+ diffp.rgb * uCol
		+ glowP.rgb
		, min(glowP.a + diffp.a, 1.0)
	);
}