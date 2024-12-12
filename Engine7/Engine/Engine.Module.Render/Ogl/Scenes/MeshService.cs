using Engine.Buffers;
using Engine.Module.Render.Ogl.Services;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Engine.Module.Render.Ogl.Scenes;

public sealed class MeshService( BufferService bufferService ) : DisposableIdentifiable {

	private readonly List<IMesh> _meshes = [];

	//public VertexMesh CreateEmptyMesh( Type vertexType, uint vertexCount, uint elementCount ) {
	//	bufferService.Get( typeof( TVertex ) ).TryAllocate( vertexCount * TypeManager.SizeOf<TVertex>(), out BufferSegment? vertexSegment );
	//	bufferService.ElementBuffer.TryAllocate( elementCount * IMesh.ElementSizeBytes, out BufferSegment? elementSegment );
	//}

	public VertexMesh<TVertex> CreateEmptyMesh<TVertex>( uint vertexCount, uint elementCount, string? name = null ) where TVertex : unmanaged {
		if (!bufferService.Get( typeof( TVertex ) ).TryAllocate( vertexCount * (uint) Marshal.SizeOf<TVertex>(), out BufferSegment? vertexSegment ))
			throw new InvalidOperationException( "Failed to allocate vertex buffer" );
		if (!bufferService.ElementBuffer.TryAllocate( elementCount * IMesh.ElementSizeBytes, out BufferSegment? elementSegment ))
			throw new InvalidOperationException( "Failed to allocate element buffer" );
		vertexSegment.Nickname = name;
		elementSegment.Nickname = name;
		VertexMesh<TVertex> mesh = new( vertexSegment, elementSegment ) {
			Nickname = name
		};
		this._meshes.Add( mesh );
		return mesh;
	}

	private void CreateEmptyReadOnlyMesh<TVertex>( uint vertexCount, uint elementCount, string? name, out ReadOnlyVertexMesh<TVertex> mesh, [NotNullIfNotNull( nameof( mesh ) )] out BufferSegment? vertexSegment, [NotNullIfNotNull( nameof( mesh ) )] out BufferSegment? elementSegment ) where TVertex : unmanaged {
		if (!bufferService.Get( typeof( TVertex ) ).TryAllocate( vertexCount * (uint) Marshal.SizeOf<TVertex>(), out vertexSegment ))
			throw new InvalidOperationException( "Failed to allocate vertex buffer" );
		if (!bufferService.ElementBuffer.TryAllocate( elementCount * IMesh.ElementSizeBytes, out elementSegment ))
			throw new InvalidOperationException( "Failed to allocate element buffer" );
		vertexSegment.Nickname = name;
		elementSegment.Nickname = name;
		mesh = new( vertexSegment, elementSegment ) {
			Nickname = name
		};
		this._meshes.Add( mesh );
	}

	public VertexMesh<TVertex> CreateMesh<TVertex>( Span<TVertex> vertices, Span<uint> elements, string? name = null ) where TVertex : unmanaged {
		VertexMesh<TVertex> mesh = CreateEmptyMesh<TVertex>( (uint) vertices.Length, (uint) elements.Length, name );
		//if (!bufferService.Get( typeof( TVertex ) ).TryAllocate( (uint) vertices.Length * (uint) Marshal.SizeOf<TVertex>(), out BufferSegment? vertexSegment ))
		//	throw new InvalidOperationException( "Failed to allocate vertex buffer" );
		//if (!bufferService.ElementBuffer.TryAllocate( (uint) elements.Length * IMesh.ElementSizeBytes, out BufferSegment? elementSegment ))
		//	throw new InvalidOperationException( "Failed to allocate element buffer" );
		//vertexSegment.Nickname = name;
		//elementSegment.Nickname = name;
		//VertexMesh<TVertex> mesh = new( vertexSegment, elementSegment ) {
		//	Nickname = name
		//};
		mesh.VertexBufferSegment.WriteRange( vertices, 0 );
		mesh.ElementBufferSegment.WriteRange( elements, 0 );
		//this._meshes.Add( mesh );
		return mesh;
	}

	public VertexMesh<TVertex> CreateMesh<TVertex>( TVertex[] vertices, uint[] elements, string? name = null ) where TVertex : unmanaged {
		VertexMesh<TVertex> mesh = CreateEmptyMesh<TVertex>( (uint) vertices.Length, (uint) elements.Length, name );
		//if (!bufferService.Get( typeof( TVertex ) ).TryAllocate( (uint) vertices.Length * (uint) Marshal.SizeOf<TVertex>(), out BufferSegment? vertexSegment ))
		//	throw new InvalidOperationException( "Failed to allocate vertex buffer" );
		//if (!bufferService.ElementBuffer.TryAllocate( (uint) elements.Length * IMesh.ElementSizeBytes, out BufferSegment? elementSegment ))
		//	throw new InvalidOperationException( "Failed to allocate element buffer" );
		//vertexSegment.Nickname = name;
		//elementSegment.Nickname = name;
		//VertexMesh<TVertex> mesh = new( vertexSegment, elementSegment ) {
		//	Nickname = name
		//};
		unsafe {
			fixed (TVertex* srcPtr = vertices)
				mesh.VertexBufferSegment.WriteRange( srcPtr, (ulong) (vertices.Length * sizeof( TVertex )), 0 );
			fixed (uint* srcPtr = elements)
				mesh.ElementBufferSegment.WriteRange( srcPtr, (ulong) (elements.Length * sizeof( uint )), 0 );
		}
		//this._meshes.Add( mesh );
		return mesh;
	}


	public ReadOnlyVertexMesh<TVertex> CreateReadOnlyMesh<TVertex>( Span<TVertex> vertices, Span<uint> elements, string? name = null ) where TVertex : unmanaged {
		CreateEmptyReadOnlyMesh( (uint) vertices.Length, (uint) elements.Length, name, out ReadOnlyVertexMesh<TVertex> mesh, out BufferSegment? vertexSegment, out BufferSegment? elementSegment );
		vertexSegment.WriteRange( vertices, 0 );
		elementSegment.WriteRange( elements, 0 );
		return mesh;
	}

	public ReadOnlyVertexMesh<TVertex> CreateReadOnlyMesh<TVertex>( TVertex[] vertices, uint[] elements, string? name = null ) where TVertex : unmanaged {
		CreateEmptyReadOnlyMesh( (uint) vertices.Length, (uint) elements.Length, name, out ReadOnlyVertexMesh<TVertex> mesh, out BufferSegment? vertexSegment, out BufferSegment? elementSegment );
		unsafe {
			fixed (TVertex* srcPtr = vertices)
				vertexSegment.WriteRange( srcPtr, (ulong) (vertices.Length * sizeof( TVertex )), 0 );
			fixed (uint* srcPtr = elements)
				elementSegment.WriteRange( srcPtr, (ulong) (elements.Length * sizeof( uint )), 0 );
		}
		return mesh;
	}

	protected override bool InternalDispose() {
		foreach (IMesh mesh in this._meshes)
			mesh.Dispose();
		return true;
	}
}
