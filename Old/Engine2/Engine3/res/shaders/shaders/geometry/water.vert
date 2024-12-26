#version 430

in vec3 vPos;
in vec2 vUV;

out vec2 oUV;
out vec4 oPos;
out vec4 oWPos;
out vec3 oToCamera;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;
uniform mat4 uVP_mat;
uniform vec3 uCameraTranslation;

void main(void){
	oUV = vUV;
	gl_Position = oPos = uMVP_mat * vec4(vPos, 1);
	oWPos = uM_mat *  vec4(vPos, 1);
	oToCamera = uCameraTranslation - oWPos.xyz;
}