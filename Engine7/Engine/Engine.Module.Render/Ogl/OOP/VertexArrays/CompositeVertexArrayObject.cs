using Engine.Logging;
using Engine.Module.Render.Ogl.OOP.Buffers;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.VertexArrays;

public sealed class CompositeVertexArrayObject : OglVertexArrayObjectBase {

	private readonly OglBufferBase _elementBuffer;
	private readonly VertexArrayLayout[] _layouts;
	private uint _numAttributeBindings;

	public CompositeVertexArrayObject( OglBufferBase elementBuffer, VertexArrayLayout[] layouts ) {
		this._elementBuffer = elementBuffer;
		this._layouts = layouts;
		this.Nickname = $"VAO{this.VertexArrayId} {string.Join( ", ", layouts.Select( p => p.BoundTo.Name ) )}";
	}

	private void AddAttrib( uint binding, int size, VertexAttribType type, bool normalized, uint relativeOffset )
		=> SetupAttrib( binding, this._numAttributeBindings++, size, type, normalized, relativeOffset );

	private void AddAttribI( uint binding, int size, VertexAttribType type, uint relativeOffset )
		=> SetupAttribI( binding, this._numAttributeBindings++, size, type, relativeOffset );

	private void AddAttribL( uint binding, int size, VertexAttribType type, uint relativeOffset )
		=> SetupAttribL( binding, this._numAttributeBindings++, size, type, relativeOffset );

	protected override void Setup() {
		this._numAttributeBindings = 0;
		for (int i = 0; i < this._layouts.Length; i++) {
			VertexArrayLayout layout = this._layouts[ i ];
			uint binding = BindBuffer( layout.Buffer.BufferId, layout.OffsetBytes, layout.StrideBytes );

			SetBindingDivisor( binding, layout.InstanceDivisor );

			for (int j = 0; j < layout.Attributes.Count; j++) {
				VertexArrayLayoutFieldData field = layout.Attributes[ j ];

				switch (field.AttributeType) {
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
						this.LogWarning( $"Unsupported {nameof( VertexArrayAttributeType )} {field.AttributeType}!" );
						break;
				}
			}
		}
		SetElementBuffer( this._elementBuffer );
		this.LogLine( $"Setup complete.", Log.Level.NORMAL );
	}
}
