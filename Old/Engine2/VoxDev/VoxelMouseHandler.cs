using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.LinearAlgebra;
using Engine.QuickstartKit;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;
using VoxDev.Voxels;

namespace VoxDev {
	public class VoxelMouseHandler : IMouseEventListener {

		private Camera3 camera;
		private VoxelWorld world;
		private float time;
		public int Radius { get; private set; }

		public VoxelMouseHandler( Camera3 camera, VoxelWorld world ) {
			this.camera = camera;
			this.world = world;
			Radius = 5;
		}

		public void ButtonHoldHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
		}

		public void ButtonPressHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
			if( btn == MouseButton.Left ) {
				if( world.TraceFrom( camera.TranformInterface.GlobalTranslation, camera.TranformInterface.GlobalRotation.Forward, 128, out TraceResult result ) ) {
					for( int x = -Radius; x <= Radius; x++ ) {
						for( int y = -Radius; y <= Radius; y++ ) {
							for( int z = -Radius; z <= Radius; z++ ) {
								if( Math.Sqrt( x * x + y * y + z * z ) <= Radius )
									world.SetId( result.BlockPosition + (x, y, z), 0 );
							}
						}
					}
				}
			}
		}

		public void ButtonReleaseHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {
		}

		public void MouseDragHandler( IntPtr window, MouseInputEventData data ) {
		}

		public void MouseMoveHandler( IntPtr window, MouseInputEventData data ) {
		}

		public void WheelScrollChangeHandler( IntPtr window, float delta, MouseInputEventData data ) {
			Radius += Math.Sign( delta );
			Radius = Math.Max( Radius, 1 );
		}

		public void CameraUpdate( InputEventHandler eventhandler ) {
			float nTime = Clock32.Standard.Time;
			if( eventhandler.Mouse[ MouseButton.Right ] && time + 0.1f < nTime) {
				time = nTime;
				if( world.TraceFrom( camera.TranformInterface.GlobalTranslation, camera.TranformInterface.GlobalRotation.Forward, 256, out TraceResult result ) ) {
					for( int x = -Radius; x <= Radius; x++ ) {
						for( int y = -Radius; y <= Radius; y++ ) {
							for( int z = -Radius; z <= Radius; z++ ) {
								if( Math.Sqrt( x * x + y * y + z * z ) <= Radius )
									world.SetId( result.BlockPosition + result.BlockFaceDirection + (x, y, z), 1 );
							}
						}
					}
				}
			}
		}
	}
}
