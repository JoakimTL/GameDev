using Engine.MemLib;
using Engine.QuickstartKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe {
	class TicTacToe : QuickstartClientRender {

		public const int BOARDSIZE = 3;
		private TicTile[,] tiles;

		/// <summary>
		/// Is it this client's turn to choose a tile?
		/// </summary>
		public bool ClientTurn { get; private set; }

		private Player localPlayer;
		private Player remotePlayer;

		public TicTacToe() : base( new QSWinData( "TicTacToe", (600, 600) ), null, new QS2Data() ) {

		}

		protected override void Initialization() {
			tiles = new TicTile[ BOARDSIZE, BOARDSIZE ];
			for( int x = 0; x < BOARDSIZE; x++ ) {
				for( int y = 0; y < BOARDSIZE; y++ ) {
					TicTile tile = new TicTile( this, x, y );
					tiles[ x, y ] = tile;
					Render2.UIManager.Add( tile, activate: true );
				}
			}

			localPlayer = new Player( Mem.Mesh2.Square, (1, 0, 0) );
			remotePlayer = new Player( Mem.Mesh2.Square, (0, 1, 0) );
		}

		protected override void InLoop() {

			StandardPipeline.Exectute();
		}

		public override void EventsPolled() {

		}

		protected override void Exit() {

		}

		internal void TrySelectTile( TicTile ticTile ) {
			ticTile.Claim( localPlayer );
		}
	}
}
