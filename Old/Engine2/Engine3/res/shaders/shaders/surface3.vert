#version 430

in vec3 vPos;

uniform mat4 uMVP_mat;

void main(void){
	gl_Position = uMVP_mat * vec4(vPos, 1);
}