#version 450

layout(location = 0) in vec3 vPos;
layout(location = 1) in vec2 vUV;
layout(location = 2) in vec3 vNor;
layout(location = 3) in vec4 vCol;

layout(location = 4) in vec3 iTranslation;
layout(location = 5) in uint iScale;
layout(location = 6) in uint iVoxelId;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out PerVertex {
	vec3 normal;
	vec4 diffuseColor;
	vec4 glowColor;
	flat float metallic;
	flat float roughness;
} OUT;

layout (std140) uniform SceneCameraBlock
{ 
	mat4 VP_mat;
	vec3 V_up;
	vec3 V_right;
} sb;

layout (std140) uniform VoxelWorldModelDataBlock
{ 
	mat4 M_mat;
} vwmdb;

struct VoxelData {
	vec4 diffuseColor;
	vec3 glowColor;
	float metallic;
	float roughness;
};

layout (std430) readonly buffer VoxelDataBlock
{
	VoxelData data[65536];
} voxels;

void main(void){
	mat4 MVP_mat = sb.VP_mat * vwmdb.M_mat;
	
	OUT.normal = normalize((vec4(vNor, 0.0)).xyz);
	OUT.diffuseColor = vec4(1);//voxels.data[iVoxelId].diffuseColor * vCol;
	OUT.glowColor = vec4(voxels.data[iVoxelId].glowColor, 1) * vCol;
	OUT.metallic = voxels.data[iVoxelId].metallic;
	OUT.roughness = voxels.data[iVoxelId].roughness;
	float scaleX = float(bitfieldExtract(iScale, 0, 5) + 1);
	float scaleY = float(bitfieldExtract(iScale, 5, 5) + 1);
	float scaleZ = float(bitfieldExtract(iScale, 10, 5) + 1);
	gl_Position = MVP_mat * vec4(iTranslation + vPos * vec3(scaleX, scaleY, scaleZ), 1.0);
}