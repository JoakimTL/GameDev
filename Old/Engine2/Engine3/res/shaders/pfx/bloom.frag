#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDiffuse;
uniform float threshold;
uniform vec3 colorWeight;
uniform float intensity;

void main(void) {
	
	vec4 c = texture(uTexDiffuse, oUV);
	vec3 rgbWeighted = (c.rgb * colorWeight);
	float t = sign(max((rgbWeighted.r + rgbWeighted.g + rgbWeighted.b) - threshold, 0));
	outColor = c * t * intensity;
	
}