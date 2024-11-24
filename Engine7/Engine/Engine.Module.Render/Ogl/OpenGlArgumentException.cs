namespace Engine.Module.Render.Ogl;
public sealed class OpenGlArgumentException( string message, params string[] parameterNames ) : Exception( $"({string.Join( ", ", parameterNames )}) {message}" );
