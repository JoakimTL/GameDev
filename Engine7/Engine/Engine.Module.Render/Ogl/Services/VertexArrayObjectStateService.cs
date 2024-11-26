namespace Engine.Module.Render.Ogl.Services;

//public sealed class VertexArrayObjectStateService( OglBufferBase elementBuffer ) : Identifiable {

//	private readonly Dictionary<Type, OglVertexArrayObjectBase> _vertexArrayObjects = [];
//	private readonly OglBufferBase _elementBuffer = elementBuffer;
//	private uint _boundVertexArrayObject = 0;

//	public T GetVertexArrayObject<T>() where T : OglVertexArrayObjectBase, new() {
//		Type t = typeof( T );
//		if (t.IsAbstract)
//			throw new OpenGlArgumentException( "Cannot create abstract vertex array object", nameof( T ) );
//		if (!_vertexArrayObjects.TryGetValue( t, out OglVertexArrayObjectBase? vao )) {
//			vao = new T();
//			vao.SetElementBuffer( _elementBuffer );
//			_vertexArrayObjects.Add( t, vao );
//		}
//		return (T) vao;
//	}

//	public void Bind( OglVertexArrayObjectBase vao ) {
//		if (vao.VertexArrayId == _boundVertexArrayObject) {
//			this.LogWarning( $"{vao} already bound" );
//			return;
//		}
//		_boundVertexArrayObject = vao.VertexArrayId;
//		vao.Bind();
//	}

//	public void Unbind() {
//		if (_boundVertexArrayObject != 0) {
//			this.LogWarning( $"There are no bound vertex array object." );
//			return;
//		}
//		_boundVertexArrayObject = 0;
//		OglVertexArrayObjectBase.Unbind();
//	}
//}