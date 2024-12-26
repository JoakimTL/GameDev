#version 430

in vec2 vPos;

in vec4 iTransform;
in vec2 iRotation;
in vec4 iTextureData;
in vec4 iDiffuse;
in vec4 iGlow;
in float iBlend;

out vec4 oPos;
out vec4 oTex;
out vec4 oDiffuse;
out vec4 oGlow;
out float oBlend;

uniform mat4 uVP_mat;
uniform vec3 lightPos;
uniform float uRows;

void main(void){
	vec2 tex = (vPos + vec2(0.5, 0.5));
	tex.y = -(1.0 - tex.y);
	tex *= uRows;
	oTex = vec4(tex + iTextureData.xy, tex + iTextureData.zw);
	oDiffuse = iDiffuse;
	oGlow = iGlow;
	oBlend = iBlend;
	vec3 toLight = normalize(iTransform.xyz - lightPos);
	vec3 right = normalize(cross(toLight, vec3(0, 1, 0)));
	vec3 up = normalize(cross(toLight, -right));
	vec2 rot = vec2(
		vPos.x * iRotation.x - vPos.y * iRotation.y,
		vPos.x * iRotation.y + vPos.y * iRotation.x
	);
	oPos = vec4(iTransform.xyz + up * rot.y * iTransform.w + right * rot.x * iTransform.w, 1.0);
	gl_Position = oPos = uVP_mat * oPos;
}