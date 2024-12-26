using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D3 {
	public class Particle3 : InstanceDatabuffered {

		public const int SIZEBYTES = 13 * sizeof( float );
		private byte[] databytes;
		public IReadOnlyList<byte> DataBytes { get => databytes; }
		public Particle3System System { get; private set; }
		public Particle3DataContainer Data { get; private set; }
		public Animation3Manager Animations { get; private set; }
		public Behaviour3Manager Behaviours { get; private set; }
		public bool Alive { get; set; }
		public bool TerminateFromQueue { get; set; }
		private bool updateNeeded;
		public float CreationTime { get; private set; }
		public float Lifetime { get; private set; }
		public float Distance { get; private set; }

		internal Particle3( Particle3System system ) {
			System = system;
			CreationTime = System.Time;
			Lifetime = 0;
			Distance = 0;
			Animations = new Animation3Manager( this );
			Behaviours = new Behaviour3Manager( this );
			Data = new Particle3DataContainer( 0 );
			databytes = new byte[ SIZEBYTES ];
			Alive = true;
			TerminateFromQueue = true;
			updateNeeded = true;
			Data.Changed += DataChanged;
		}

		internal void SetDistance( Camera3 camera ) {
			Distance = ( camera.TranformInterface.GlobalTranslation - Data.Translation ).LengthSquared;
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
					Data.Diffuse = 0;
				if( updateNeeded ) {
					updateNeeded = false;
					Data.FillByteData( Lifetime, databytes );
				}
			}
			return Alive;
		}
	}
}
