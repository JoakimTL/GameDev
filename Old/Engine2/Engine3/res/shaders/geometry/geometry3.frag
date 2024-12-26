#version 430

in vec4 oPos;
in vec4 oCol;
in vec3 oNorT;
in vec2 oUV;

uniform vec4 uColor;
uniform float uRoughness;
uniform float uMetallic;
uniform float uDiffuseIntensity;
uniform float uLighting;
uniform float uNormalMapped;

uniform sampler2D cTexture;
uniform sampler2D nTexture;
uniform sampler2D sTexture;
uniform sampler2D gTexture;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec3 outNormal;
layout(location = 2) out vec3 outLightInfo;
layout(location = 3) out vec4 outGlow;
layout(location = 4) out vec4 outParticles;

void main(void) {
	if (oCol.a * uColor.a == 0.0)
		discard;
	outColor = texture(cTexture, oUV) * oCol * uColor;
	outNormal = normalize(oNorT + (texture(nTexture, oUV).rgb * 2.0 - 1.0) * uNormalMapped) * 0.5 + 0.5;
	outLightInfo = vec3(vec2(uMetallic, uRoughness) * texture(sTexture, oUV).rg, uLighting);
	outGlow = texture(gTexture, oUV) * oCol * uColor;
	outParticles = vec4(0.0, 0.0, 0.0, 0.0);
}