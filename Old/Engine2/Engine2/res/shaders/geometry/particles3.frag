#version 430

in vec4 oPos;
in vec4 oTex;
in vec4 oColor;
in float oBlend;

uniform sampler2D uTexture;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec4 outNormal;
layout(location = 2) out vec2 outSpecular;
layout(location = 3) out vec4 outGlow;
layout(location = 4) out vec4 outParticles;

void main(void){
	outColor = vec4(0.0);
	outNormal = vec4(0.0);
	outSpecular = vec2(0.0);
	outGlow = vec4(0.0);
	outParticles = mix(texture(uTexture, oTex.xy), texture(uTexture, oTex.zw), oBlend) * oColor;
}