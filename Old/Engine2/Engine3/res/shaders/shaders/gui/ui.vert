#version 430

in vec2 vPos;
in vec3 vCol;
in vec2 vUV;

out vec4 oCol;
out vec4 oPos;
out vec2 oUV;

uniform mat4 uMVP_mat;

void main(void){
	oCol = vec4(vCol, 1);
	oUV = vUV;
	gl_Position = oPos = uMVP_mat * vec4(vPos, 0, 1);
}