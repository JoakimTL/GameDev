#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in PerVertex {
	vec4 Position;
	vec2 Uv;
	vec4 Color;
	float FillNegativeAnchor;
	float FillPositiveAnchor;
	vec3 FillQuadratic;
	float FillGradient;
} IN;

//Let's map vUv.x to a with this function: a = (2 * (iFillLayout.x + a * (iFillLayout.y - iFillLayout.x) - iFillLayout.z) / (iFillLayout.w - iFillLayout.z)) - 1
//The line is filled if vUv.y < f(vUv.x) {iFillQuadratic.x * iFillQuadratic.x * a + iFillQuadratic.y * a + iFillQuadratic.z}

//Alpha is a factor if iFillQuadratic.w is greater than 0. At 0 the separation between fill and unfilled is at f(x). iFillQuadratic.w is the width of the gradient between filled and unfilled.

layout(location = 0) out vec4 outColor;

float findQuadraticEquationX(float x, float positiveAnchor, float negativeAnchor) {
    return 2 * (x - (positiveAnchor + negativeAnchor) * 0.5) / (positiveAnchor - negativeAnchor);
}

float quadratic(float x, float quad, float linear, float constant) {
	return quad * x * x + linear * x + constant;
}

void main(void) {
//	float quadEqX = findQuadraticEquationX(IN.Uv.x, IN.FillAnchors.y, IN.FillAnchors.x);
//	float quadraticResult = quadratic(quadEqX, IN.FillQuadratic.x, IN.FillQuadratic.y, IN.FillQuadratic.z);
//	if (IN.Uv.y * sign(quadraticResult) > quadraticResult) {
//		discard;
//	}
	outColor = IN.Color;
	//IN.Uv.x -> UV on width
	//IN.Uv.y -> UV on length
	//IN.Fill.x < Uv.Y
}