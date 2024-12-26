#version 430

in vec2 vPos;

out vec2 oUV;

void main(void){
	oUV = vPos / 2 + .5f;
	gl_Position = vec4(vPos, 0, 1);
}