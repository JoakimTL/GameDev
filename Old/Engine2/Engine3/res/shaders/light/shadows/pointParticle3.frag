#version 430

in vec4 oPos;
in vec4 oTex;
in vec4 oDiffuse;
in vec4 oGlow;
in float oBlend;

out vec4 diffuseData;

uniform sampler2D uTexture;

void main(void) {
	diffuseData = mix(texture(uTexture, oTex.xy), texture(uTexture, oTex.zw), oBlend) * vec4(1 - oDiffuse.rgb, oDiffuse.a);
	diffuseData.a *= diffuseData.a;
}