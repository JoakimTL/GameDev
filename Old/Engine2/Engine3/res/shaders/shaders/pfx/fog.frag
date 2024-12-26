#version 430

in vec2 oUV;

out vec4 outColor;

uniform sampler2D uTexDepth;

uniform float zNear;
uniform float zFar;
uniform float uFOV;
uniform float uDepth;
uniform vec3 uColor;

float depth(in vec2 coo) {
    float z_b = float(texture2D( uTexDepth, coo ));
	float z_n = 2.0 * z_b - 1.0;
    return 2.0 * zNear * zFar / (zFar + zNear - z_n * (zFar - zNear));
}

void main(void) {
	float depthSample = depth( oUV );
	outColor = vec4(uColor, (depthSample * depthSample * depthSample) / (uDepth * uDepth * uDepth));
}