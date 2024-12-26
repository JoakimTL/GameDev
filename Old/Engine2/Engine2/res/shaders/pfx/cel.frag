#version 430

in vec2 oUV;

layout(location = 0) out vec4 outColor;

uniform sampler2D uTexture;
uniform float uSteps;

void main(void) {
	vec4 col = texture(uTexture, oUV);
	float inverse = 1.0 / uSteps;
	outColor = vec4(
		round(col.x * uSteps) * inverse, 
		round(col.y * uSteps) * inverse, 
		round(col.z * uSteps) * inverse, 
		col.w
	);
}