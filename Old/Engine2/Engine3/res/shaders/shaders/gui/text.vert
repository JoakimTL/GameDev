#version 430

in vec2 vPos;
in vec2 iTranslation;
in vec2 iSize;
in vec2 iUVOffset;
in vec2 iUVSize;
in vec4 iCol;

out vec4 oPos;
out vec2 oUV;
out vec4 oCol;

uniform mat4 uMVP_mat;

void main(void){
	oUV = vPos * iUVSize + iUVOffset;
	gl_Position = oPos = uMVP_mat * vec4(vPos * iSize + iTranslation, 0, 1);
	oCol = iCol;
}