namespace Engine.Module.Render.OpenGL.Ogl;
public sealed class OpenGlArgumentException( string message, params string[] parameterNames ) : Exception( $"({string.Join( ", ", parameterNames )}) {message}" );
