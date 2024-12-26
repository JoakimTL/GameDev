using Engine.MemLib;
using Engine.Utilities.Data;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Graphics.Objects {
	public abstract class Mesh : Cacheable, ICountable {

		private static readonly List<IReadOnlyList<uint>> sequences = new List<IReadOnlyList<uint>>() { new List<uint>() };

		public uint VAO { get; private set; }
		private uint id;
		public uint ID { get => GetID(); }
		private bool needsAssignment;
		protected ShaderBind shaderBind;
		protected Dictionary<uint, DataArray<byte>.DataSegment> DataSegments { get; private set; }
		private bool allocationChanged;

		public Mesh( string name ) : base( name ) {
			DataSegments = new Dictionary<uint, DataArray<byte>.DataSegment>();
			id = 0;
			shaderBind = null;
			AllocateVAO();
		}

		public void AllocateVAO() {
			VAO = Gl.GenVertexArray();
			Logging.Memory( $"[{Name}]: Allocated VAO at index {VAO}" );
		}

		protected uint AllocateBuffer( int s, StorageType storeType, BufferUsage bufferUse ) {
			if( s < 0 )
				return 0;
			DataArray<byte>.DataSegment seg = Mem.Meshes.AllocateSpace( out uint bufferId, s, storeType, bufferUse );
			DataSegments.Add( bufferId, seg );
			seg.OffsetChanged += OffsetChanged;
			needsAssignment = true;
			return bufferId;
		}

		private void OffsetChanged() {
			allocationChanged = true;
		}

		protected void LoadFloats( uint buffer, int offset, IReadOnlyList<float> data ) {
			if( !DataSegments.TryGetValue( buffer, out DataArray<byte>.DataSegment dataSegment ) ) {
				Mem.Logs.Warning.WriteLine( $"[{Name}] Mesh reference is null." );
				return;
			}
			byte[] dataBytes = DataTransform.GetBytes( data );
			dataSegment.SetRange( offset, dataBytes );
		}

		protected void LoadInts( uint buffer, int offset, IReadOnlyList<int> data ) {
			if( !DataSegments.TryGetValue( buffer, out DataArray<byte>.DataSegment dataSegment ) ) {
				Mem.Logs.Warning.WriteLine( $"[{Name}] Mesh reference is null." );
				return;
			}
			byte[] dataBytes = DataTransform.GetBytes( data );
			dataSegment.SetRange( offset, dataBytes );
		}

		public override void Dispose() {
			if( VAO > 0 ) {
				Gl.DeleteVertexArrays( VAO );
				foreach( DataArray<byte>.DataSegment segs in DataSegments.Values ) {
					segs.Dispose();
				}
				DataSegments.Clear();
				needsAssignment = true;
				Mem.Logs.MemoryLogger.WriteLine( $"[{Name}]: Disposed [{VAO}]!" );
			}
			VAO = 0;
		}

		private uint GetID() {
			if( needsAssignment )
				id = AssignID();
			return id;
		}

		private uint AssignID() {
			lock( sequences ) {
				needsAssignment = false;
				IReadOnlyList<uint> sequence = DataSegments.Keys.ToList();
				if( sequence.Count == 0 )
					return 0;

				for( int i = 0; i < sequences.Count; i++ ) {
					if( sequences[ i ].SequenceEqual( sequence ) )
						return (uint) i;
				}

				sequences.Add( sequence );
				return (uint) sequences.Count - 1;
			}
		}

		public override int GetHashCode() {
			return (int) ID;
		}

		public override bool Equals( object obj ) {
			if( !( obj is Mesh o ) )
				return false;
			return Equals( o );
		}

		public bool Equals( Mesh obj ) {
			return obj.ID == ID && obj.VAO == VAO;
		}

		public static bool operator ==( Mesh a, Mesh b ) {
			if( a is null || b is null )
				return false;
			return a.Equals( b );
		}

		public static bool operator !=( Mesh a, Mesh b ) {
			return !( a == b );
		}

		public void PreBind() {
			if( allocationChanged )
				Setup( VAO );
		}

		public void BindShader( Shader s ) => shaderBind?.Invoke( s );
		public abstract void Setup( uint VAO );
		public abstract void Bind();
		public abstract void RenderMesh();
		public abstract void Unbind();

	}
}
