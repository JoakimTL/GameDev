#version 430

in vec3 vPos;
in vec3 vNor;
in vec2 vUV;
in vec3 iTranslation;
in vec3 iScale;
in vec2 iUV;

out vec3 oPos;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;

void main(void){
	vec4 pos = vec4(vPos * iScale + iTranslation, 1.0);
	oPos = (uM_mat * pos).xyz;
	gl_Position = uMVP_mat * pos;
}