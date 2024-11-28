using Engine.Logging;
using Engine.Module.Render.Ogl.OOP.Buffers;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Processing;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Engine.Module.Render.Ogl.Services;

[Do<IInitializable>.After<OglBufferService>]
public sealed class VertexArrayLayoutService( OglBufferService vertexBufferObjectService ) : Identifiable, IInitializable {

	private readonly OglBufferService _vertexBufferObjectService = vertexBufferObjectService ?? throw new ArgumentNullException( nameof( vertexBufferObjectService ) );
	private readonly Dictionary<Type, VertexArrayLayout> _layouts = [];

	public void Initialize() {
		List<VertexArrayLayoutFieldData> fields = [];

		foreach (Type layoutType in TypeManager.WithAttribute<VAO.SetupAttribute>( TypeManager.Registry.ValueTypes )) {
			StructLayoutAttribute? structLayout = layoutType.StructLayoutAttribute;
			if (structLayout is null) {
				this.LogWarning( $"{layoutType.Name} is missing the {nameof( StructLayoutAttribute )}! Skipping." );
				continue;
			}

			if (structLayout.Value != LayoutKind.Explicit) {
				this.LogWarning( $"{layoutType.Name} has a {nameof( StructLayoutAttribute )}, but the layout is not {nameof( LayoutKind.Explicit )}! Skipping." );
				continue;
			}

			VAO.SetupAttribute vaoSetup = layoutType.GetCustomAttribute<VAO.SetupAttribute>() ?? throw new NullReferenceException( $"Couldn't get {nameof( VAO.SetupAttribute )} for {layoutType.Name}!" );

			int strideBytes = vaoSetup.StrideBytesOverride >= 0
				? vaoSetup.StrideBytesOverride
				: Marshal.SizeOf( layoutType ) + vaoSetup.TextureCount * sizeof( ushort );

			OglBufferBase vbo = this._vertexBufferObjectService.Get( layoutType );

			fields.Clear();

			foreach (FieldInfo field in layoutType.GetFields()) {
				if (!field.FieldType.IsValueType) {
					this.LogWarning( $"{layoutType.Name}.{field.Name} is not a value type! Skipping." );
					continue;
				}

				VAO.DataAttribute? data = field.GetCustomAttribute<VAO.DataAttribute>();
				if (data is null) {
					this.LogWarning( $"{layoutType.Name}.{field.Name} is missing the {nameof( VAO.DataAttribute )}! Skipping." );
					continue;
				}

				FieldOffsetAttribute? fieldOffset = field.GetCustomAttribute<FieldOffsetAttribute>();
				if (fieldOffset is null) {
					this.LogWarning( $"{layoutType.Name}.{field.Name} is missing the {nameof( FieldOffsetAttribute )}! Skipping." );
					continue;
				}

				uint vertices = data.VertexCount;
				int offsetBytes = data.RelativeOffsetBytesOverride >= 0 ? data.RelativeOffsetBytesOverride : fieldOffset.Value;
				int sizeOfField = TypeManager.SizeOf( field.FieldType ) ?? throw new NullReferenceException( $"Couldn't get size of {layoutType.Name}.{field.Name}!" );
				uint sizePerVertex = (uint) sizeOfField / vertices;

				if (sizePerVertex == 0)
					this.LogWarning( $"Bytesize per vertex for {layoutType.Name}.{field.Name} is {sizePerVertex}!" );
				if (sizePerVertex > 4 && data.AttributeType != VertexArrayAttributeType.LARGE)
					this.LogWarning( $"Bytesize per vertex for {layoutType.Name}.{field.Name} is {sizePerVertex} while not using the {nameof( VertexArrayAttributeType.LARGE )} {nameof( VertexArrayAttributeType )}!" );

				while (vertices > 0) {
					uint addedVertices = Math.Min( vertices, 4 );

					fields.Add( new VertexArrayLayoutFieldData( data.VertexAttributeType, addedVertices, (uint) offsetBytes, data.AttributeType, data.Normalized ) );
					offsetBytes += (int) (addedVertices * sizePerVertex);
					vertices -= addedVertices;
				}
			}

			int offset = Marshal.SizeOf( layoutType );
			for (int tex = 0; tex < vaoSetup.TextureCount; tex += 4) {
				int addedTextures = Math.Min( vaoSetup.TextureCount - tex, 4 );
				fields.Add( new VertexArrayLayoutFieldData( OpenGL.VertexAttribType.UnsignedShort, (uint) addedTextures, (uint) offset, VertexArrayAttributeType.INTEGER, false ) );
				offset += addedTextures * sizeof( ushort );
			}
			this._layouts.Add( layoutType, new VertexArrayLayout( layoutType, vbo, vaoSetup.OffsetBytes, strideBytes, vaoSetup.InstanceDivisor, fields ) );
		}
	}

	public VertexArrayLayout? Get( Type t ) => this._layouts.GetValueOrDefault( t );
}
