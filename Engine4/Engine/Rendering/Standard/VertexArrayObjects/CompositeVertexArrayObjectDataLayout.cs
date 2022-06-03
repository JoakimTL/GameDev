namespace Engine.Rendering.Standard.VertexArrayObjects;

public abstract class CompositeVertexArrayObjectDataLayout : Identifiable {

	public VertexBufferObject Buffer { get; private set; }
	public uint OffsetBytes { get; private set; }
	public int StrideBytes { get; private set; }
	public uint InstanceDivisor { get; private set; }
	private readonly List<AttributeDataLayout> _attributes;
	public IReadOnlyList<AttributeDataLayout> Attributes => this._attributes;

	public CompositeVertexArrayObjectDataLayout( string name, VertexBufferObject buffer, uint offsetBytes, int strideBytes, uint instanceDivisor ) : base( name ) {
		this.Buffer = buffer;
		this.OffsetBytes = offsetBytes;
		this.StrideBytes = strideBytes;
		this.InstanceDivisor = instanceDivisor;
		this._attributes = new List<AttributeDataLayout>();
	}

	protected void AddAttribute( AttributeDataLayout attribute ) => this._attributes.Add( attribute );
}
