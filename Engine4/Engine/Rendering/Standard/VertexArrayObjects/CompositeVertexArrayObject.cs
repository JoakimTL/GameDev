using OpenGL;

namespace Engine.Rendering.Standard.VertexArrayObjects;
public sealed class CompositeVertexArrayObject : VertexArrayObject {

	private readonly CompositeVertexArrayObjectDataLayout[] _layouts;
	private uint _numAttributeBindings;

	private CompositeVertexArrayObject( CompositeVertexArrayObjectDataLayout[] layoutTypes ) : base( $"Composite VAO[{string.Join( ", ", layoutTypes.Select( p => p.TypeName ) )}]" ) {
		this._numAttributeBindings = 0;
		this._layouts = layoutTypes;
	}

	private void AddAttrib( uint binding, int size, VertexAttribType type, bool normalized, uint relativeOffset )
		=> SetupAttrib( binding, this._numAttributeBindings++, size, type, normalized, relativeOffset );

	private void AddAttribI( uint binding, int size, VertexAttribType type, uint relativeOffset )
		=> SetupAttribI( binding, this._numAttributeBindings++, size, type, relativeOffset );

	private void AddAttribL( uint binding, int size, VertexAttribType type, uint relativeOffset )
		=> SetupAttribL( binding, this._numAttributeBindings++, size, type, relativeOffset );

	protected override void Setup() {
		for ( int i = 0; i < this._layouts.Length; i++ ) {
			CompositeVertexArrayObjectDataLayout? layout = this._layouts[ i ];

			uint layoutBinding = AddBuffer( layout.Buffer, layout.OffsetBytes, layout.StrideBytes );

			if ( layout.InstanceDivisor != 0 )
				SetBindingDivisor( layoutBinding, layout.InstanceDivisor );

			for ( int j = 0; j < layout.Attributes.Count; j++ ) {
				AttributeDataLayout attrib = layout.Attributes[ j ];
				switch ( attrib.AttributeType ) {
					case AttributeType.DEFAULT:
						AddAttrib( layoutBinding, attrib.VertexCount, attrib.VertexAttributeType, attrib.Normalized, attrib.RelativeOffsetBytes );
						break;
					case AttributeType.INTEGER:
						AddAttribI( layoutBinding, attrib.VertexCount, attrib.VertexAttributeType, attrib.RelativeOffsetBytes );
						break;
					case AttributeType.LARGE:
						AddAttribL( layoutBinding, attrib.VertexCount, attrib.VertexAttributeType, attrib.RelativeOffsetBytes );
						break;
					default:
						this.LogError( "Unsupported AttributeType!" );
						;
						break;
				}
			}
		}
		SetElementBuffer( Resources.Render.VBOs.ElementBuffer );
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="vaoParts">Types such as Vertex3, Vector3, Vector2, etc...</param>
	/// <returns></returns>
	public static CompositeVertexArrayObject? Create( Span<Type> vaoParts ) {
		List<CompositeVertexArrayObjectDataLayout> layoutList = new();
		for ( int i = 0; i < vaoParts.Length; i++ ) {
			CompositeVertexArrayObjectDataLayout? layout = Resources.Render.VAOLayoutBindings.GetLayout( vaoParts[ i ] );
			if ( layout is not null )
				layoutList.Add( layout );
		}
		if ( layoutList.Count == vaoParts.Length )
			return new CompositeVertexArrayObject( layoutList.ToArray() );
		Log.Warning( $"Failed creating constructed VAO from { string.Join( ", ", vaoParts.ToArray().Select( p => p.Name ) ) }" );
		return null;
	}
}
