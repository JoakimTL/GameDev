#version 430

in vec2 vPos;
in vec4 iTra;
in vec4 iUV;
in vec4 iCol;

out vec2 oUV;
out vec4 oCol;

uniform mat4 uMVP_mat;

void main(void){
	oUV = vPos * iUV.zw + iUV.xy;
	gl_Position = uMVP_mat * vec4(vPos * iTra.zw + iTra.xy, 0, 1);
	oCol = iCol;
}