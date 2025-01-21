#version 450
#extension GL_ARB_bindless_texture : require
#extension GL_ARB_gpu_shader_int64 : require

layout(location = 0) in vec2 vPos;
layout(location = 1) in vec2 vUv;
layout(location = 2) in vec4 vColor;
layout(location = 3) in vec4 iPointA;			//xyz, w = width
layout(location = 4) in vec4 iPointB;			//xyz, w = width
layout(location = 5) in vec3 iLineNormal;		//xyz
//layout(location = 7) in vec4 iFill;			//Rectangle on the uv which is filled. x = xStart, y = yStart, z = xEnd, w = yEnd
layout(location = 6) in vec2 iFillAnchors;		//Where x is the negativeAnchor and y is the positiveAnchor
layout(location = 7) in vec4 iFillQuadratic;	//x = quadratic, y = linear, z = constant, w = gradientSharpness. The quadratic equation y is on the width of the line, while the x = 0 is at (iFill.z + iFill.x) / 2, and x = -1 at iFillLayout.x, and x = 1 at iFillLayout.z. 
layout(location = 8) in vec2 iDistanceGradient;
layout(location = 9) in vec4 iColorStart;
layout(location = 10) in vec4 iColorEnd;

//Let's map vUv.x to a with this function: a = (2 * (iFillLayout.x + a * (iFillLayout.y - iFillLayout.x) - iFillLayout.z) / (iFillLayout.w - iFillLayout.z)) - 1
//The line is filled if vUv.y < f(vUv.x) {iFillQuadratic.x * iFillQuadratic.x * a + iFillQuadratic.y * a + iFillQuadratic.z}

//Alpha is a factor if iFillQuadratic.w is greater than 0. At 0 the separation between fill and unfilled is at f(x). iFillQuadratic.w is the width of the gradient between filled and unfilled.

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PerVertex {
	vec4 Position;
	vec2 Uv;
	vec4 Color;
	float FillNegativeAnchor;
	float FillPositiveAnchor;
	vec3 FillQuadratic;
	float FillGradient;
	float InvisibleAtDistance;
	float DistanceGradient;
} OUT;

layout (std140) uniform SceneCameraBlock
{ 
	mat4 VP_mat;
	vec3 V_up;
	vec3 V_right;
} sb;

void main(void){
//	mat4 MVP_mat = sb.VP_mat * iM_mat;

	vec3 pointA = iPointA.xyz;
	vec3 pointB = iPointB.xyz;

	vec3 dir = normalize(pointB - pointA);
	vec3 right = normalize(cross(dir, iLineNormal));

	//Create a quasi billboard out of the vertices with the help of iPointA and iPointB

	float width = ((1 - vUv.y) * iPointA.w + vUv.y * iPointB.w);
	vec3 pos = vPos.x * right * width + vPos.y * (pointB - pointA) + pointA;

	OUT.Position = sb.VP_mat * vec4(pos, 1.0);
	OUT.Uv = vUv;
	OUT.Color = vColor * (iColorStart * (1 - vUv.y) +iColorEnd * vUv.y);
	OUT.FillNegativeAnchor = iFillAnchors.x;
	OUT.FillPositiveAnchor = iFillAnchors.y;
	OUT.FillQuadratic = iFillQuadratic.xyz;
	OUT.FillGradient = iFillQuadratic.w;
	OUT.InvisibleAtDistance = iDistanceGradient.x;
	OUT.DistanceGradient = iDistanceGradient.y;
	gl_Position = OUT.Position;
}