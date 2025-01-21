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
	float InvisibleAtDistance;
	float DistanceGradient;
} IN;

layout(location = 0) out vec4 outColor;

float findQuadraticEquationX(float x, float positiveAnchor, float negativeAnchor) {
    return 2 * (x - (positiveAnchor + negativeAnchor) * 0.5) / (positiveAnchor - negativeAnchor);
}

float quadratic(float x, float quad, float linear, float constant) {
	return quad * x * x + linear * x + constant;
}

void main(void) {
	float quadEqX = findQuadraticEquationX(IN.Uv.y, IN.FillNegativeAnchor, IN.FillPositiveAnchor);
	float quadraticResult = quadratic(quadEqX, IN.FillQuadratic.x, IN.FillQuadratic.y, IN.FillQuadratic.z);
	if (IN.Uv.x > quadraticResult) {
		discard;
	}
	float distanceFromEdge = quadraticResult - IN.Uv.x;
//	float distanceFromCamera = IN.DistanceGradient * IN.Position.z;
	float clamped = clamp(distanceFromEdge / IN.FillGradient, 0, 1);
	outColor = vec4(IN.Color.r, IN.Color.g, IN.Color.b, IN.Color.a * clamped);
	//clamp((IN.Uv.x - quadraticResult + 1) * .5 / IN.FillGradient, 0, 1);
}