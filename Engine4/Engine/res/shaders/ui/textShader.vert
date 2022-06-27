#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec2 vPos;

layout(location = 1) in mat4 iM_mat;
layout(location = 5) in uint64_t iFontTexture;
layout(location = 6) in vec2 iTranslation;
layout(location = 7) in vec2 iScale;
layout(location = 8) in vec2 iRotation;
layout(location = 9) in vec4 iUV;
layout(location = 10) in vec4 iColor;
layout(location = 11) in vec2 iGlyphData;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out Vertex {
    vec4 Color;
    vec2 UV;
    float Thickness;
    float Edge;
	flat uint64_t FontTexture;
} OUT;

layout (std140) uniform SceneCameraBlock
{ 
	mat4 VP_mat;
	vec3 V_up;
	vec3 V_right;
} sb;

void main(){
	mat4 MVP_mat = sb.VP_mat * iM_mat;
	vec2 uv = (vPos + 1) * .5;
	OUT.UV = iUV.xy + iUV.zw * vec2(uv.x, 1 - uv.y);
	OUT.Color = vec4(iColor.rgb * .5, iColor.a);
	OUT.Thickness = iGlyphData.x;
	OUT.Edge = iGlyphData.y;
	OUT.FontTexture = iFontTexture;
	vec2 pos = vPos * iScale;
	vec2 rot = vec2(
		pos.x * iRotation.x - pos.y * iRotation.y,
		pos.x * iRotation.y + pos.y * iRotation.x
	);
	gl_Position = MVP_mat * vec4(vec3(iTranslation.xy, 0.0) + sb.V_up * rot.y + sb.V_right * rot.x, 1.0);
}