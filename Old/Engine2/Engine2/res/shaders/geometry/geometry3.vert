#version 430

in vec3 vPos;
in vec3 vCol;
in vec3 vNor;
in vec2 vUV;

out vec4 oCol;
out vec3 oNorT;
out vec4 oPos;
out vec2 oUV;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;
uniform mat4 uVP_mat;

uniform float ambient;

uniform vec4 clipPlane;

void main(void){
	oCol = vec4(vCol, 1.0);
	oNorT = normalize((uM_mat * vec4(vNor, 0.0)).xyz);
	oPos = vec4(vPos, 1.0);
	oUV = vUV;
	gl_ClipDistance[0] = dot(clipPlane, uM_mat * oPos);
	gl_Position = uMVP_mat * oPos;
	oPos = gl_Position;
}