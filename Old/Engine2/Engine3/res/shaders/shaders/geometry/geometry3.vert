#version 430

in vec3 vPos;
in vec3 vNor;
in vec2 vUV;
in vec3 vCol;

out vec4 oCol;
out vec3 oNorT;
out vec4 oPos;
out vec2 oUV;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;

void main(void){
	oCol = vec4(vCol, 1.0);
	oNorT = normalize((uM_mat * vec4(vNor, 0.0)).xyz);
	oUV = vUV;
	oPos = uMVP_mat * vec4(vPos, 1.0);
	gl_Position = oPos;
}