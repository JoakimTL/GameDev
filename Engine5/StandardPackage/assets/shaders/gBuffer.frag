#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PFXVertex {
    vec2 uv;
} IN;

layout(location = 0) out vec4 OUT;

layout (std140) uniform PFXBlock
{ 
	uvec2 diffuse;
	uvec2 accum;
	uvec2 reveal;
} pfx;

// epsilon number
const float EPSILON = 0.00001f;

// calculate floating point numbers equality accurately
bool isApproximatelyEqual(float a, float b)
{
    return abs(a - b) <= (abs(a) < abs(b) ? abs(b) : abs(a)) * EPSILON;
}

// get the max value between three values
float max3(vec3 v)
{
    return max(max(v.x, v.y), v.z);
}

void main() {
	sampler2D diffuseTex = sampler2D(pfx.diffuse);
	sampler2D accumTex = sampler2D(pfx.accum);
	sampler2D revealTex = sampler2D(pfx.reveal);

    // fragment revealage
    float revealage = texture(revealTex, IN.uv).r;

    // fragment color
    vec4 accumulation = texture(accumTex, IN.uv);

    // suppress overflow
    if (isinf(max3(abs(accumulation.rgb))))
        accumulation.rgb = vec3(accumulation.a);

    // prevent floating point precision bug
    vec3 average_color = accumulation.rgb / max(accumulation.a, EPSILON);

    OUT = vec4(texture(diffuseTex, IN.uv).rgb * revealage + average_color * (1.0f - revealage), 1.0);
}