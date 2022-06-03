#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec3 vTranslation;
layout(location = 1) in vec3 iColor;
layout(location = 2) in float iIntensity;
layout(location = 3) in float iRadius;
layout(location = 4) in vec3 iTranslation;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PFXVertex {
    flat vec3 color;
    flat float intensity;
    flat float radius;
    flat vec3 translation;
} OUT;

layout (std140) uniform SceneCameraBlock
{ 
	mat4 VP_mat;
	vec3 V_up;
	vec3 V_right;
} sb;

void main() {
	OUT.color = iColor;
	OUT.intensity = iIntensity;
	OUT.radius = iRadius;
	OUT.translation = iTranslation;
    gl_Position = sb.VP_mat * vec4(iTranslation + vTranslation * iRadius, 1);
}