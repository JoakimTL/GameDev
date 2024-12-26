#version 430

in vec2 oUV;
in vec4 oCol;

out vec4 outColor;

uniform sampler2D uDiffuse;
uniform vec4 uColor;

void main(void) {
	
	float d = 1 - texture(uDiffuse, oUV).a;
	
	float mixRatio = smoothstep(.475, .625, d);
	float alpha = 1 - smoothstep(.575, .65, d);
	
	outColor = vec4(0);
	outColor += vec4( mix(uColor.xyz * oCol.rgb, 1 - uColor.xyz * oCol.rgb, mixRatio), alpha);
	outColor.a *= uColor.a * oCol.a;
}