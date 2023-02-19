using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Rendering.Objects.VAOs;
public class CompositeVertexArrayObject : VertexArrayObjectBase {

	private readonly VertexBufferObject _elementBuffer;
	private readonly VertexArrayLayout[] _layouts;
	private uint _numAttributeBindings;

	protected override string UniqueNameTag => $"{base.UniqueNameTag}[{_numAttributeBindings}]";

	public CompositeVertexArrayObject( VertexBufferObject elementBuffer, VertexArrayLayout[] layouts ) : base( string.Join( ", ", layouts.Select( p => p.BoundTo.Name ) ) ) {
		this._elementBuffer = elementBuffer;
		this._layouts = layouts;
	}

	private void AddAttrib( uint binding, int size, VertexAttribType type, bool normalized, uint relativeOffset )
		=> SetupAttrib( binding, this._numAttributeBindings++, size, type, normalized, relativeOffset );

	private void AddAttribI( uint binding, int size, VertexAttribType type, uint relativeOffset )
		=> SetupAttribI( binding, this._numAttributeBindings++, size, type, relativeOffset );

	private void AddAttribL( uint binding, int size, VertexAttribType type, uint relativeOffset )
		=> SetupAttribL( binding, this._numAttributeBindings++, size, type, relativeOffset );

	protected override void Setup() {
		_numAttributeBindings = 0;
		for ( int i = 0; i < _layouts.Length; i++ ) {
			var layout = _layouts[ i ];
			uint binding = AddBuffer( layout.Buffer, layout.OffsetBytes, layout.StrideBytes );

			if ( layout.InstanceDivisor != 0 )
				SetBindingDivisor( binding, layout.InstanceDivisor );

			for ( int j = 0; j < layout.Attributes.Count; j++ ) {
				var field = layout.Attributes[ j ];

				switch ( field.AttributeType ) {
					case VertexArrayAttributeType.DEFAULT:
						AddAttrib( binding, (int) field.VertexCount, field.VertexAttributeType, field.Normalized, field.RelativeOffsetBytes );
						break;
					case VertexArrayAttributeType.INTEGER:
						AddAttribI( binding, (int) field.VertexCount, field.VertexAttributeType, field.RelativeOffsetBytes );
						break;
					case VertexArrayAttributeType.LARGE:
						AddAttribL( binding, (int) field.VertexCount, field.VertexAttributeType, field.RelativeOffsetBytes );
						break;
					default:
						this.LogError( $"Unsupported {nameof( VertexArrayAttributeType )} {field.AttributeType}!" );
						break;
				}
			}
		}
		SetElementBuffer( _elementBuffer );
		this.LogLine( $"Setup complete.", Log.Level.NORMAL );
	}
}
