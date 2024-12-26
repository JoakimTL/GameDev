#version 430

in vec4 oPos;
in vec3 oNorT;
in vec2 oUV;
in vec2 oUVStart;

uniform float uRoughness;
uniform float uMetallic;
uniform float uDiffuseIntensity;
uniform float uLighting;
uniform float uNormalMapped;

uniform sampler2D uTexDiffuse;
uniform sampler2D uTexNormal;
uniform sampler2D uTexLighting;
uniform sampler2D uTexGlow;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec3 outNormal;
layout(location = 2) out vec3 outLightInfo;
layout(location = 3) out vec4 outGlow;
layout(location = 4) out vec4 outParticles;
void main(void) {
	vec2 uv = oUVStart + mod(oUV, 1.0)/256;
	uv.y = 1 - uv.y;
	outColor = texture(uTexDiffuse, uv);
	outNormal = normalize(oNorT + (texture(uTexNormal, uv).rgb * 2.0 - 1.0) * uNormalMapped) * 0.5 + 0.5;
	outLightInfo = vec3(vec2(uMetallic, uRoughness) * texture(uTexLighting, uv).rg, uLighting);
	outGlow = texture(uTexGlow, uv);
	outParticles = vec4(0.0, 0.0, 0.0, 0.0);
}