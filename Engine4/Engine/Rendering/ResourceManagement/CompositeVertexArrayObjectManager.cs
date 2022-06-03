using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Engine.Data;
using Engine.Rendering.Standard.VertexArrayObjects;

namespace Engine.Rendering.ResourceManagement;

[Structure.ProcessBefore( typeof( Window ), typeof( IDisposable ) )]
public class CompositeVertexArrayObjectManager : DisposableIdentifiable {
	private readonly Dictionary<string, CompositeVertexArrayObject> _vaos;

	public CompositeVertexArrayObjectManager() {
		this._vaos = new();
	}

	public bool TryGet<A, B, C, D>( [NotNullWhen( returnValue: true )] out CompositeVertexArrayObject? vao )
		=> TryGet( new Type[] { typeof( A ), typeof( B ), typeof( C ), typeof( D ) }, out vao );

	public bool TryGet<A, B, C>( [NotNullWhen( returnValue: true )] out CompositeVertexArrayObject? vao )
		=> TryGet( new Type[] { typeof( A ), typeof( B ), typeof( C ) }, out vao );

	public bool TryGet<A, B>( [NotNullWhen( returnValue: true )] out CompositeVertexArrayObject? vao )
		=> TryGet( new Type[] { typeof( A ), typeof( B ) }, out vao );

	public bool TryGet<A>( [NotNullWhen( returnValue: true )] out CompositeVertexArrayObject? vao )
		=> TryGet( new Type[] { typeof( A ) }, out vao );

	public bool TryGet( Span<Type> subparts, [NotNullWhen( returnValue: true )] out CompositeVertexArrayObject? vao ) {
		vao = null;
		string? idString = GetIdentifyingString( subparts );
		if ( idString is null )
			return false;
		if ( this._vaos.TryGetValue( idString, out vao ) )
			return true;
		vao = CompositeVertexArrayObject.Create( subparts );
		if ( vao is null )
			return false;
		this._vaos.Add( idString, vao );
		return true;
	}

	private unsafe string? GetIdentifyingString( Span<Type> input ) {
		Span<Guid> guids = stackalloc Guid[ input.Length ];
		for ( int i = 0; i < guids.Length; i++ )
			guids[ i ] = input[ i ].GUID;
		Span<byte> bytes = stackalloc byte[ guids.Length * sizeof( Guid ) ];
		fixed ( Guid* srcPtr = guids )
		fixed ( byte* dstPtr = bytes )
			Unsafe.CopyBlock( dstPtr, srcPtr, (uint) bytes.Length );
		return DataUtils.ToStringUTF8( bytes );
	}

	protected override bool OnDispose() {
		foreach ( VertexArrayObject vao in this._vaos.Values )
			vao.Dispose();
		this._vaos.Clear();
		return this._vaos.Count == 0;
	}
}
