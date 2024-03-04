namespace OpenGL.OOP;

public class OglTexture( ContextWarningLog warningLog, TextureTarget target, Vector2ui level0, InternalFormat format, params (TextureParameterName, int)[] parameters ) 
	: OglMipmappedTexture( warningLog, target, level0, 1, format, parameters );
