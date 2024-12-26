#version 430

in vec3 vPos;
in vec3 vCol;

out vec4 oCol;
out vec4 oPos;

uniform mat4 uMVP_mat;

void main(void){
	oCol = vec4(vCol, 1.0);
	gl_Position = oPos = uMVP_mat * vec4(vPos, 1.0);
}