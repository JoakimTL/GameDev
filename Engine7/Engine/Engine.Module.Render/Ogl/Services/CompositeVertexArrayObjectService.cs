using Engine.Logging;
using Engine.Module.Render.Ogl.OOP.VertexArrays;

namespace Engine.Module.Render.Ogl.Services;

public sealed class CompositeVertexArrayObjectService( OglBufferService vertexBufferObjectService, VertexArrayLayoutService vertexArrayLayoutService ) : Identifiable {

	private readonly OglBufferService _vertexBufferObjectService = vertexBufferObjectService;
	private readonly VertexArrayLayoutService _vertexArrayLayoutService = vertexArrayLayoutService;
	private readonly Dictionary<string, CompositeVertexArrayObject> _cvaos = [];

	public CompositeVertexArrayObject? Get( params Span<Type> vaoParts ) {
		string? id = GetIdentifyingString( vaoParts );
		if (id is null)
			return null;
		if (this._cvaos.TryGetValue( id, out CompositeVertexArrayObject? cvao ))
			return cvao;
		VertexArrayLayout[] layouts = new VertexArrayLayout[ vaoParts.Length ];
		for (int i = 0; i < vaoParts.Length; i++) {
			VertexArrayLayout? layout = this._vertexArrayLayoutService.Get( vaoParts[ i ] );
			if (layout is null) {
				this.LogWarning( $"Couldn't get layout for {vaoParts[ i ]}!" );
				return null;
			}
			layouts[ i ] = layout;
		}
		cvao = new CompositeVertexArrayObject( this._vertexBufferObjectService.ElementBuffer, layouts );
		this._cvaos.Add( id, cvao );
		return cvao;
	}

	private static unsafe string? GetIdentifyingString( Span<Type> input ) {
		if (input.Length == 0)
			return null;
		Guid* srcPtr = stackalloc Guid[ input.Length ];
		for (int i = 0; i < input.Length; i++)
			srcPtr[ i ] = input[ i ].GUID;
		return new string( new Span<char>( srcPtr, input.Length * sizeof( Guid ) ) );
	}
}
