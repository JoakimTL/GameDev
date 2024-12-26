#version 430

in vec3 vPos;
in vec3 vCol;
in vec2 vUV;

out vec4 oCol;
out vec4 oPos;
out vec2 oUV;

uniform mat4 uMVP_mat;

void main(void){
	oCol = vec4(vCol, 1);
	oPos = vec4(vPos, 1);
	oUV = vUV;
	gl_Position = uMVP_mat * oPos;
}