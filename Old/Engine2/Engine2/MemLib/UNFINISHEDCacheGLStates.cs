using Engine.Utilities.Data;
using Engine.Utilities.Graphics.Utilities;
using OpenGL;
using System;
using System.Collections.Generic;

namespace Engine.MemLib {
	public class UNFINISHEDCacheGLStates {

		private BitSet availableStates;

		private Dictionary<int, bool> defaultCaps;
		private Dictionary<int, Dictionary<int, bool>> stateCaps;

		public int CurrentState { get; private set; }
		public int PushedState { get; private set; }

		public UNFINISHEDCacheGLStates( int maxAvailableStates = 8192 ) {
			availableStates = new BitSet( maxAvailableStates );
			availableStates.Set( 0 );
			defaultCaps = new Dictionary<int, bool>();
			stateCaps = new Dictionary<int, Dictionary<int, bool>>();
		}

		public bool IsEnabled(EnableCap cap) {
			if( defaultCaps.TryGetValue( (int) cap, out bool a ) )
				return a;
			return false;
		}

		public int CreateState( params StateCap[] caps ) {
			int index = availableStates.Min;
			if( index == -1 )
				return index;
			Dictionary<int, bool> dict = new Dictionary<int, bool>();
			foreach( StateCap sc in caps ) {
				dict[ (int) sc.Cap ] = sc.State;
			}
			stateCaps[ index ] = dict;
			return index;
		}

		public void SetState( int stateId ) {
			if( !stateCaps.TryGetValue( stateId, out Dictionary<int, bool> state ) )
				throw new ArgumentOutOfRangeException( "reference from stateId not found!" );

			if( CurrentState > 0 )
				ResetState();

			foreach( KeyValuePair<int, bool> sCap in state ) {
				if( sCap.Value )
					Gl.Enable( (EnableCap) sCap.Key );
				else
					Gl.Disable( (EnableCap) sCap.Key );
			}

			CurrentState = stateId;
		}

		public void Push( int newState ) {
			PushedState = CurrentState;
			SetState( newState );
		}

		public void Pop() {
			if( PushedState > 0 )
				SetState( PushedState );
			PushedState = -1;
		}

		private void ResetState() {
			if( !stateCaps.TryGetValue( CurrentState, out Dictionary<int, bool> state ) ) {
				Mem.Logs.Warning.WriteLine( "Couldn't find current active GL state!" );
				return;
			}

			foreach( KeyValuePair<int, bool> sCap in state ) {
				bool v = defaultCaps[ sCap.Key ];
				if( sCap.Value != v )
					if( defaultCaps[ sCap.Key ] )
						Gl.Enable( (EnableCap) sCap.Key );
					else
						Gl.Disable( (EnableCap) sCap.Key );
			}
		}

		internal void Initialize() {
			foreach( EnableCap cap in Enum.GetValues( typeof( EnableCap ) ) ) {
				defaultCaps[ (int) cap ] = Gl.IsEnabled( cap );
				GLUtil.CheckError( cap.ToString(), true );
			}
		}
	}

	public struct StateCap {
		public EnableCap Cap { get; private set; }
		public bool State { get; private set; }

		public StateCap( EnableCap cap, bool state ) {
			Cap = cap;
			State = state;
		}
	}
}
