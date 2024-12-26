#version 430

in vec3 vPos;
in vec2 vUV;

out vec2 oUV;

uniform mat4 uMVP_mat;

void main(void) {
	oUV = vUV;
	gl_Position = uMVP_mat * vec4(vPos, 1);
}