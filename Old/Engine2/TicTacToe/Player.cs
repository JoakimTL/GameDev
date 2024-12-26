using Engine.Graphics.Objects;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe {
	class Player {

		/// <summary>
		/// The texture displayed on captured tiles to indicate which player owns it. Usually a circle or X.
		/// </summary>
		public Mesh ClaimantMesh { get; private set; }

		public Vector3 Color { get; private set; }

		public Player( Mesh claimMesh, Vector3 color ) {
			ClaimantMesh = claimMesh;
			Color = color;
		}

	}
}
