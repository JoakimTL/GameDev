#version 430

in vec3 vPos;
in vec2 vUV;

out vec3 oPos;
out vec2 oUV;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;
uniform mat4 uVP_mat;

void main(void){
	vec4 pos = vec4(vPos, 1);
	gl_Position = uMVP_mat * pos;
	oPos = (uM_mat * pos).xyz;
	oUV = vUV;
}