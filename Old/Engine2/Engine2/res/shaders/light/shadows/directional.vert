#version 430

in vec3 vPos;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;
uniform mat4 uVP_mat;

void main(void){
	gl_Position = uMVP_mat * vec4(vPos, 1);
}