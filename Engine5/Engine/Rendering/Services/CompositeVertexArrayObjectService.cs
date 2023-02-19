using Engine.Data;
using Engine.Rendering.Objects.VAOs;
using System.Runtime.CompilerServices;

namespace Engine.Rendering.Services;

public class CompositeVertexArrayObjectService : Identifiable, IContextService {
	private readonly VertexBufferObjectService _vertexBufferObjectService;
	private readonly VertexArrayLayoutService _vertexArrayLayoutService;
	private readonly Dictionary<string, CompositeVertexArrayObject> _cvaos;

	public CompositeVertexArrayObjectService( VertexBufferObjectService vertexBufferObjectService, VertexArrayLayoutService vertexArrayLayoutService ) {
		this._vertexBufferObjectService = vertexBufferObjectService;
		this._vertexArrayLayoutService = vertexArrayLayoutService;
		_cvaos = new();
	}

	public CompositeVertexArrayObject? Get( Span<Type> vaoParts ) {
		string? id = GetIdentifyingString( vaoParts );
		if ( id is null )
			return null;
		if ( _cvaos.TryGetValue( id, out var cvao ) )
			return cvao;
		var layouts = new VertexArrayLayout[ vaoParts.Length ];
		for ( int i = 0; i < vaoParts.Length; i++ ) {
			var layout = _vertexArrayLayoutService.Get( vaoParts[ i ] );
			if ( layout is null ) {
				this.LogWarning( $"Couldn't get layout for {vaoParts[ i ]}!" );
				return null;
			}
			layouts[ i ] = layout;
		}
		cvao = new CompositeVertexArrayObject( _vertexBufferObjectService.ElementBuffer, layouts );
		_cvaos.Add( id, cvao );
		return cvao;
	}

	private static unsafe string? GetIdentifyingString( Span<Type> input ) {
		if ( input.Length == 0 )
			return null;
		Guid* srcPtr = stackalloc Guid[ input.Length ];
		for ( int i = 0; i < input.Length; i++ )
			srcPtr[ i ] = input[ i ].GUID;
		byte* dstPtr = stackalloc byte[ input.Length * sizeof( Guid ) ];
		Unsafe.CopyBlock( dstPtr, srcPtr, (uint) ( input.Length * sizeof( Guid ) ) );
		return DataExtensions.CreateString( dstPtr, (uint) ( input.Length * sizeof( Guid ) ) );
	}
}
