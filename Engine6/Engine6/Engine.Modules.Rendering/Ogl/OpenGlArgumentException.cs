namespace Engine.Modules.Rendering.Ogl;
public sealed class OpenGlArgumentException( string message, params string[] parameterNames ) : Exception( $"({string.Join( ", ", parameterNames )}) {message}" );
