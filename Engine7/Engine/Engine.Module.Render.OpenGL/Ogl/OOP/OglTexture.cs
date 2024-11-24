using OpenGL;

namespace Engine.Module.Render.OpenGL.Ogl.OOP;

public class OglTexture( string name, TextureTarget target, Vector2<int> level0, InternalFormat format, params (TextureParameterName, int)[] parameters )
	: OglMipmappedTexture( name, target, level0, 1, format, parameters );
