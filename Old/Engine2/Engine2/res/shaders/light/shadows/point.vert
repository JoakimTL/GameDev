#version 430

in vec3 vPos;

out vec3 oPos;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;
uniform mat4 uVP_mat;

void main(void){
	vec4 pos = vec4(vPos, 1);
	gl_Position = uMVP_mat * pos;
	oPos = (uM_mat * pos).xyz;
}