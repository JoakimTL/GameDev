namespace OpenGL;
public sealed class OpenGlArgumentException( string message, params string[] parameterNames ) : Exception( $"({string.Join(", ", parameterNames)}) {message}" );
