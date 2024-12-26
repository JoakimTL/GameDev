using Engine.Utilities.Data;
using Engine.Utilities.Graphics.Utilities;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Engine.MemLib {
	public class StoreMesh {

		private readonly int defaultSize;
		private readonly Dictionary<BufferUsage, Dictionary<StorageType, List<InternalMesh>>> meshes;
		private readonly HashSet<InternalMesh> meshSet;

		public StoreMesh( int defaultSize ) {
			this.defaultSize = defaultSize;
			meshes = new Dictionary<BufferUsage, Dictionary<StorageType, List<InternalMesh>>>();
			meshSet = new HashSet<InternalMesh>();
			foreach( BufferUsage bufferType in Enum.GetValues( typeof( BufferUsage ) ) ) {
				Dictionary<StorageType, List<InternalMesh>> dict = new Dictionary<StorageType, List<InternalMesh>>();
				meshes.Add( bufferType, dict );
				foreach( StorageType storeType in Enum.GetValues( typeof( StorageType ) ) )
					dict.Add( storeType, new List<InternalMesh>() );
			}
		}

		public DataArray<byte>.DataSegment AllocateSpace( out uint bufferIndex, int size, StorageType storeType, BufferUsage usage = BufferUsage.DynamicDraw ) {
			bufferIndex = 0;
			lock( meshes ) {
				List<InternalMesh> list = meshes[ usage ][ storeType ];
				DataArray<byte>.DataSegment @out = null;
				int num = list.Count;
				for( int i = 0; i < num; i++ ) {
					@out = list[ i ].AllocateSpace( size );
					if( !( @out is null ) ) {
						bufferIndex = list[ i ].BO;
						return @out;
					}
				}

				//no mesh with enough space found, let's make a new one!
				InternalMesh mesh = null;
				if( size > defaultSize ) {
					mesh = new InternalMesh( size, usage );
				} else
					mesh = new InternalMesh( defaultSize, usage );
				mesh.Initialize();
				list.Add( mesh );
				meshSet.Add( mesh );
				bufferIndex = mesh.BO;
				return mesh.AllocateSpace( size );
			}
		}

		public void Update() {
			foreach( InternalMesh mesh in meshSet ) {
				mesh.Update();
			}
		}

	}

	public enum StorageType {
		VERTEXDATA, INDEXDATA
	}

	public class InternalMesh {

		public uint BO { get; private set; }
		public BufferUsage Usage { get; private set; }
		private readonly DataArray<byte> data;
		private volatile bool resized;
		private readonly Queue<DataArray<byte>.DataChangeSegment> dataChanges;

		public InternalMesh( int sizeBytes, BufferUsage usage ) {
			Usage = usage;
			data = new DataArray<byte>( sizeBytes, false );
			dataChanges = new Queue<DataArray<byte>.DataChangeSegment>();
			data.Resized += ResizeEvent;
			data.DataChange += DataChangeEvent;
			data.DataReset += DataResetEvent;
		}

		private void DataResetEvent() {
			lock( dataChanges )
				dataChanges.Clear();
		}

		private void DataChangeEvent( DataArray<byte>.DataChangeSegment seg ) {
			lock( dataChanges )
				dataChanges.Enqueue( seg );
		}

		private void ResizeEvent() {
			resized = true;
		}

		internal void Initialize() {
			BO = Gl.GenBuffer();
			Gl.BindBuffer( BufferTarget.ArrayBuffer, BO );
			Gl.BufferData( BufferTarget.ArrayBuffer, data.Size, (IntPtr) 0, Usage );
		}

		public void Update() {
			lock( dataChanges ) {
				if( resized ) {
					Gl.BindBuffer( BufferTarget.ArrayBuffer, BO );
					Gl.BufferData( BufferTarget.ArrayBuffer, data.Size, data.Data, Usage );
					dataChanges.Clear();
				} else {
					if( dataChanges.Count == 0 )
						return;
					Gl.BindBuffer( BufferTarget.ArrayBuffer, BO );
					unsafe {
						lock( data.Data ) {
							fixed( byte* p = data.Data ) {
								while( dataChanges.TryDequeue( out DataArray<byte>.DataChangeSegment seg ) )
									Gl.BufferSubData( BufferTarget.ArrayBuffer, (IntPtr) seg.Offset, (uint) seg.Length, (IntPtr) ( p + seg.Offset ) ); //might not work
							}
						}
					}
				}
			}
		}

		public DataArray<byte>.DataSegment AllocateSpace( int size ) {
			return data.CreateSegment( size );
		}
	}
}
