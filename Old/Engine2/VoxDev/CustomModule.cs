using Engine.Entities;
using Engine.Entities.D3;
using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;

namespace VoxDev {
	class CustomModule : Module, IKeyboardEventListener {

		private Camera3 camera;
		private Transform3 cameraTransform;
		private InputEventHandler input;
		private float defaultSpeed;
		private bool toggle;

		public CustomModule( Camera3 camera, Transform3 cameraTransform, InputEventHandler input, float defaultSpeed ) {
			this.camera = camera;
			this.cameraTransform = cameraTransform;
			this.input = input;
			this.defaultSpeed = defaultSpeed;
			input.Keyboard.Add( this );
		}

		public override void Update( float time, float deltaTime ) {
			if( camera.HasParent ) {
				if( Entity.Get( out CustomRigidbodyModule rbm ) && Entity.Get( out Transform3Module tm ) ) {
					float speed = defaultSpeed;
					if( toggle && !rbm.Colliding )
						toggle = false;

					if( input.Keyboard[ Keys.Space ] )
						if( rbm.Colliding && !toggle ) {
							rbm.ApplyForce( tm.Transform.Rotation.Up * 1000 );
							toggle = true;
						}
					if( input.Keyboard[ Keys.W ] ) {
						rbm.ApplyForce( tm.Transform.Rotation.Forward * speed );
					}
					if( input.Keyboard[ Keys.A ] ) {
						rbm.ApplyForce( tm.Transform.Rotation.Left * speed );
					}
					if( input.Keyboard[ Keys.S ] ) {
						rbm.ApplyForce( tm.Transform.Rotation.Backward * speed );
					}
					if( input.Keyboard[ Keys.D ] ) {
						rbm.ApplyForce( tm.Transform.Rotation.Right * speed );
					}
				}
			} else {
				float speed = defaultSpeed;
				if( input.Keyboard[ Keys.Space ] )
					speed *= 1.5f;
				if( input.Keyboard[ Keys.W ] ) {
					camera.TranformInterface.Translation += ( camera.TranformInterface.Rotation.Forward * speed * deltaTime );
				}
				if( input.Keyboard[ Keys.A ] ) {
					camera.TranformInterface.Translation += ( camera.TranformInterface.Rotation.Left * speed * deltaTime );
				}
				if( input.Keyboard[ Keys.S ] ) {
					camera.TranformInterface.Translation += ( camera.TranformInterface.Rotation.Backward * speed * deltaTime );
				}
				if( input.Keyboard[ Keys.D ] ) {
					camera.TranformInterface.Translation += ( camera.TranformInterface.Rotation.Right * speed * deltaTime );
				}
				if( input.Keyboard[ Keys.Space ] ) {
					camera.TranformInterface.Translation += ( camera.TranformInterface.Rotation.Up * speed * deltaTime );
				}
				if( input.Keyboard[ Keys.LeftControl ] ) {
					camera.TranformInterface.Translation += ( camera.TranformInterface.Rotation.Down * speed * deltaTime );
				}
			}
		}

		public void KeyReleaseHandler( IntPtr window, Keys key, ModifierKeys mods ) {
			if( key == Keys.C ) {
				if( Mem.Windows.Get( window, out GLWindow glWindow ) ) {
					input.Mouse.SetLock( glWindow.GLFWWindow, !input.Mouse.Data.Locked );
				}
			}
			if( key == Keys.Escape )
				if( Mem.Windows.Get( window, out GLWindow glWindow ) ) {
					input.Mouse.SetLock( glWindow.GLFWWindow, false );
				}

			if( key == Keys.R ) {
				if( camera.HasParent ) {
					camera.TranformInterface.Translation = cameraTransform.GlobalTranslation;
					camera.TranformInterface.Rotation = cameraTransform.GlobalRotation;
					camera.SetParent( null );
				} else {
					camera.TranformInterface.Translation = 0;
					camera.TranformInterface.Rotation = Quaternion.Identity;
					camera.SetParent( cameraTransform );
				}
			}
		}

		public void KeyPressHandler( IntPtr window, Keys key, ModifierKeys mods ) {
		}

		public void WritingHandler( IntPtr window, char c ) {
		}

		protected override void Initialize() {

		}
	}
}
