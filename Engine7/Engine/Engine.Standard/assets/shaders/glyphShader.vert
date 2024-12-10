#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec2 vPos;
layout(location = 1) in vec2 vUV;
layout(location = 2) in vec4 vColor;
layout(location = 3) in vec2 vLetterInformation;
layout(location = 4) in mat4 iM_mat;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PerVertex {
	vec4 Position;
	vec2 UV;
	vec4 Color;
	bool Fill;
	bool Flip;
} OUT;

void main(void){
	OUT.Position = iM_mat * vec4(vPos.x, vPos.y, 0.0, 1.0);
	OUT.UV = vUV;
	OUT.Color = vColor;
	OUT.Fill = vLetterInformation.x != 0;
	OUT.Flip = vLetterInformation.y != 0;
	gl_Position = OUT.Position;
}