using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2 {
	public class Particle2 : InstanceDatabuffered {

		public const int SIZEBYTES = 11 * sizeof( float );
		private byte[] databytes;
		public IReadOnlyList<byte> DataBytes { get => databytes; }
		public Particle2System System { get; private set; }
		public Particle2DataContainer Data { get; private set; }
		public Animation2Manager Animations { get; private set; }
		public Behaviour2Manager Behaviours { get; private set; }
		public bool Alive { get; set; }
		public bool TerminateFromQueue { get; set; }
		private bool updateNeeded;
		public float CreationTime { get; private set; }
		public float Lifetime { get; private set; }

		internal Particle2( Particle2System system ) {
			System = system;
			CreationTime = System.Time;
			Lifetime = 0;
			Animations = new Animation2Manager( this );
			Behaviours = new Behaviour2Manager( this );
			Data = new Particle2DataContainer( 0 );
			databytes = new byte[ SIZEBYTES ];
			Alive = true;
			TerminateFromQueue = true;
			updateNeeded = true;
			Data.Changed += DataChanged;
		}

		private void DataChanged() {
			updateNeeded = true;
		}

		public override bool Update() {
			if( Alive ) {
				Lifetime = System.Time - CreationTime;
				Animations.Update();
				Behaviours.Update();
				if( !Alive )
					Data.Color = 0;
				if( updateNeeded ) {
					updateNeeded = false;
					Data.FillByteData( databytes );
				}
			}
			return Alive;
		}
	}
}
