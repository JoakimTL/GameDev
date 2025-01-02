using Engine.Logging;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Processing;

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
		int identificationStringLength = 0;
		for (int i = 0; i < input.Length; i++) {
			ResolvedType resolvedType = TypeManager.ResolveType( input[ i ] );
			if (resolvedType.Identity is null)
				throw new InvalidOperationException( $"Type {input[ i ].Name} has no identity!" );
			identificationStringLength += resolvedType.Identity.Length;
		}
		Span<char> identification = stackalloc char[ identificationStringLength ];
		int index = 0;
		for (int i = 0; i < input.Length; i++) {
			ResolvedType resolvedType = TypeManager.ResolveType( input[ i ] );
			resolvedType.Identity!.CopyTo( identification[ index.. ] );
			index += resolvedType.Identity.Length;
		}
		return new string( identification );
	}
}
