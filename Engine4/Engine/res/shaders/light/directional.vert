#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec2 vTranslation;
layout(location = 1) in vec3 iColor;
layout(location = 2) in float iIntensity;
layout(location = 3) in vec3 iDirection;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PFXVertex {
    vec2 uv;
    flat vec3 color;
    flat float intensity;
    flat vec3 direction;
} OUT;

void main() {
	OUT.uv = (vTranslation + 1) * .5;
	OUT.color = iColor;
	OUT.intensity = iIntensity;
	OUT.direction = iDirection;
    gl_Position = vec4(vTranslation, 0, 1);
}