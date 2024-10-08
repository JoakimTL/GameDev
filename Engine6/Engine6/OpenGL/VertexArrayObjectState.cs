﻿namespace OpenGL;

using OpenGL.OOP;

public sealed class VertexArrayObjectState( ContextWarningLog warningLog, OglBufferBase elementBuffer ) {

	private readonly Dictionary<Type, OglVertexArrayObjectBase> _vertexArrayObjects = [];
	private readonly ContextWarningLog _warningLog = warningLog ?? throw new ArgumentNullException( nameof( warningLog ) );
	private readonly OglBufferBase _elementBuffer = elementBuffer;
	private uint _boundVertexArrayObject = 0;

	public T GetVertexArrayObject<T>() where T : OglVertexArrayObjectBase, new() {
		var t = typeof( T );
		if (t.IsAbstract)
			throw new OpenGlArgumentException( "Cannot create abstract vertex array object", nameof( T ) );
		if (!_vertexArrayObjects.TryGetValue( t, out OglVertexArrayObjectBase? vao )) {
			vao = new T();
			vao.SetElementBuffer( _elementBuffer );
			_vertexArrayObjects.Add( t, vao );
		}
		return (T) vao;
	}

	public void Bind( OglVertexArrayObjectBase vao ) {
		if (vao.VertexArrayId == _boundVertexArrayObject) {
			_warningLog.Equals( $"{vao} already bound" );
			return;
		}
		_boundVertexArrayObject = vao.VertexArrayId;
		vao.Bind();
	}

	public void Unbind() {
		if (_boundVertexArrayObject != 0) {
			_warningLog.Equals( $"There are no bound vertex array object." );
			return;
		}
		_boundVertexArrayObject = 0;
		OglVertexArrayObjectBase.Unbind();
	}
}