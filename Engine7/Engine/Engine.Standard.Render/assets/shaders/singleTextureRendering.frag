#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec4 Position;
	vec2 UV;
} IN;

layout(location = 0) out vec4 outColor;

layout (std140) uniform SingleTextureRenderingBlock
{ 
	uvec2 textureId;
} strb;

void main(void) {
	sampler2D diffuseTex = sampler2D(strb.textureId);
	outColor = texture(diffuseTex, IN.UV);
}