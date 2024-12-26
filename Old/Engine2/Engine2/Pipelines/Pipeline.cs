using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Pipelines {
	public class Pipeline {

		public string Name { get; private set; }
		private Dictionary<uint, Junction> junctionDictionary;
		private HashSet<uint> reserved;
		private List<Junction> junctionsSorted;
		private int length;

		public event Action PreExecutionHandler;
		public event Action PostExecutionHandler;
		public event Action PreJunctionHandler;
		public event Action PostJunctionHandler;

		public Pipeline( string name ) {
			Name = name;
			junctionDictionary = new Dictionary<uint, Junction>();
			reserved = new HashSet<uint>();
			junctionsSorted = new List<Junction>();
			length = 0;
		}

		public void Exectute() {
			PreExecutionHandler?.Invoke();
			for( int i = 0; i < length; i++ ) {
				Junction j = junctionsSorted[ i ];
				if( j.Active ) {
					PreJunctionHandler?.Invoke();
					j.Effect?.Invoke();
					PostJunctionHandler?.Invoke();
				}
			}
			PostExecutionHandler?.Invoke();
		}

		public void Insert( uint i, Junction j, bool active = true ) {
			if( i == 0 ) {
				Mem.Logs.Error.WriteLine( $"[{Name}] Index for [{j}] cannot be 0." );
				return;
			}

			if( junctionDictionary.ContainsKey( i ) ) {
				Mem.Logs.Error.WriteLine( $"[{Name}] Unable to insert [{j}] into slot [{i}]." );
				return;
			}

			j.SetID( i );
			j.SetActive( active );
			junctionDictionary.Add( i, j );
			reserved.Remove( i );

			SortJunctions();
		}

		public void InsertLast( Junction j ) => Insert( FindLast(), j );

		public uint FindLast() {
			uint i = 1;
			if( length > 0 )
				i = junctionsSorted[ length - 1 ].ID + 1;
			while( reserved.Contains( i ) )
				i++;
			return i;
		}

		private void SortJunctions() {
			junctionsSorted.Clear();
			junctionsSorted.AddRange( junctionDictionary.Values );
			junctionsSorted.Sort( JunctionSort );
			length = junctionsSorted.Count;
		}

		private int JunctionSort( Junction a, Junction b ) {
			if( a.ID < b.ID )
				return -1;
			return 1;
		}

		public override string ToString() {
			return $"[Pipeline][{Name}:{length}]";
		}
	}
}
