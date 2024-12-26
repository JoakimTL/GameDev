#version 430

in vec2 vPos;

in mat4 iM_Mat;
in vec4 iTex;
in vec4 iColor;
in float iBlend;

out vec4 oPos;
out vec4 oTex;
out vec4 oColor;
out float oBlend;

uniform mat4 uVP_mat;
uniform int rows;

uniform vec4 clipPlane;

void main(void){
	vec2 tex = (vPos + vec2(0.5, 0.5));
	tex.y = -(1.0 - tex.y);
	tex /= rows;
	oTex = vec4(tex + iTex.xy, tex + iTex.zw);
	oColor = iColor;
	oBlend = iBlend;
	oPos = iM_Mat * vec4(vPos, 0.0, 1.0);
	gl_ClipDistance[0] = dot(clipPlane, oPos);
	gl_Position = oPos = uVP_mat * oPos;
}