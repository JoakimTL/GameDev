#version 430

in vec3 vPos;
in vec3 vNor;
in vec2 vUV;
in vec3 iTranslation;
in vec3 iScale;
in vec2 iUV;

out vec3 oNorT;
out vec4 oPos;
out vec2 oUV;
out vec2 oUVStart;

uniform mat4 uMVP_mat;
uniform mat4 uM_mat;

void main(void){
	oNorT = normalize((uM_mat * vec4(vNor, 0.0)).xyz);
	oPos = vec4(vPos * iScale + iTranslation, 1.0);
	vec2 scale = iScale.xy * (1 - sign(iScale.z)) + iScale.xz * (1 - sign(iScale.y)) + iScale.yz * (1 - sign(iScale.x));
	oUV = vUV * scale;
	oUVStart = iUV;
	oPos = gl_Position = uMVP_mat * oPos;
}