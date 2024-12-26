#version 430

in vec2 vPos;

in vec3 iTransform;
in vec2 iRotation;
in vec4 iTextureData;
in vec4 iColor;
in float iBlend;

out vec4 oPos;
out vec4 oTex;
out vec4 oColor;
out float oBlend;

uniform mat4 uVP_mat;
uniform float uRows;

void main(void){
	vec2 tex = (vPos + vec2(0.5, 0.5));
	tex.y = -(1.0 - tex.y);
	tex *= uRows;
	oTex = vec4(tex + iTextureData.xy, tex + iTextureData.zw);
	oColor = iColor;
	oBlend = iBlend;
	vec2 rot = vec2(
		vPos.x * iRotation.x - vPos.y * iRotation.y,
		vPos.x * iRotation.y + vPos.y * iRotation.x
	);
	oPos = vec4(rot * iTransform.z + iTransform.xy, 0.0, 1.0);
	gl_Position = oPos = uVP_mat * oPos;
}