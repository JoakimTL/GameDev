namespace Engine.Module.Render;

public class Class1 {

}

//Stuff needed to render a model on screen:
//	Render data
//		Transformation matrix
//		Uniform color?
//	Model Vertex Mesh
//	Material
//		Shader
//		Textures


// To render we need to know about the following from the entity it relates to:
//	The entity transformation
//	The entity's future transformation
//	The entity's model
//	The entity's material

// Maybe the render thread should have it's own entity container, which holds "entities" that is currently being rendered.
// The render thread can have it's own render logic for these entities. For standard engine stuff that would be loading and unloading assets based on the entity it is attached to.
// Other more specific render logic would be rendering voxels for "chunk" entities, which would entail creating a mesh from the voxel data and then rendering it.

// So the system would be an entity container which looks for a "ShouldRender" component on entities, and then creates a render entity which contains a reference to the original entity.
//	This render entity then contains the relevant render data for the entity.
//	Updates and such must be handled thread safely.
