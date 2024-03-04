using Engine.Data;
using Engine.OpenGL;
using OpenGL;

namespace Engine.OpenGL.OOP;

public class OglTexture( ContextWarningLog warningLog, TextureTarget target, Vector2i level0, InternalFormat format, params (TextureParameterName, int)[] parameters )
	: OglMipmappedTexture( warningLog, target, level0, 1, format, parameters );
