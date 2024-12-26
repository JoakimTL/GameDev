#version 430

in vec4 oPos;
in vec4 oTex;
in vec4 oDiffuse;
in vec4 oGlow;
in float oBlend;
in vec2 viewportCoord;

uniform sampler2D uDiffTexture;
uniform sampler2D uDepTex;

uniform float zNear;
uniform float zFar;

float linearDepth(float depthSample)
{
    depthSample = 2.0 * depthSample - 1.0;
    float zLinear = 2.0 * zNear * zFar / (zFar + zNear - depthSample * (zFar - zNear));
    return zLinear;
}

layout(location = 0) out vec4 outDiffuse;
layout(location = 1) out vec4 outGlow;
layout(location = 2) out float outDepth;

void main(void){
	outDepth = gl_FragCoord.z;
	float dep = texture(uDepTex, viewportCoord).r;
	if (gl_FragCoord.z < dep){
		vec4 diff = mix(texture(uDiffTexture, oTex.xy), texture(uDiffTexture, oTex.zw), oBlend);
		if (diff.a == 0)
			discard;
		outDiffuse = diff * oDiffuse;
		outGlow = diff * oGlow;
		float smoothing = smoothstep(0.00, 1.0, 1 - exp(-(linearDepth(dep) - linearDepth(gl_FragCoord.z))));
		outDiffuse.a *= smoothing;
		outGlow.a *= smoothing;
	}
}