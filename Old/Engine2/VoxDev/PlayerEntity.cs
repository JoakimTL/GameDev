using Engine.Entities;
using Engine.Entities.D3;
using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Materials;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using VoxDev.Voxels;

namespace VoxDev {
	public class PlayerEntity : Entity, IMouseEventListener {

		private Transform3 cameraTransform;

		public Camera3 Camera { get; private set; }

		public PlayerEntity( string name, Camera3 camera, InputEventHandler eventHandler, Scene<SceneObjectData3> scene, VoxelWorld world ) : base( name ) {
			Transform3Module tm = new Transform3Module();
			tm.Transform.Scale = (0.3f, 0.3f, 0.3f);
			Add( tm );
			cameraTransform = new Transform3();
			cameraTransform.SetParent( tm.Transform );
			cameraTransform.Translation = (0, 0.8f, 0);
			Camera = camera;
			Camera.SetParent( cameraTransform );
			Render3Module rm = new Render3Module(
				Mem.Mesh3.Icosphere,
				new PBRMaterial( "blank" )
					.AddTexture( OpenGL.TextureUnit.Texture0, Mem.Textures.BlankWhite )
					.AddTexture( OpenGL.TextureUnit.Texture1, Mem.Textures.BlankBlack )
					.AddTexture( OpenGL.TextureUnit.Texture2, Mem.Textures.BlankWhite )
					.AddTexture( OpenGL.TextureUnit.Texture3, Mem.Textures.BlankWhite ),
				Mem.ShaderBundles.Entity3
			);
			Add( rm );
			scene.Add( rm.RenderObject );
			Mem.CollisionMolds.IcosphereUniform.MoldNew( rm.RenderObject.Data.CollisionModel );
			eventHandler.Mouse.Add( this );
			Add( new CustomModule( camera, cameraTransform, eventHandler, 4 ) );
			Add( new CustomRigidbodyModule( world, rm.RenderObject.Data.CollisionModel ) );
		}

		public void ButtonReleaseHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {

		}

		public void ButtonPressHandler( IntPtr window, MouseButton btn, ModifierKeys modifier, MouseInputEventData data ) {

		}

		public void MouseMoveHandler( IntPtr window, MouseInputEventData data ) {

		}

		public void MouseDragHandler( IntPtr window, MouseInputEventData data ) {
			if( Camera.HasParent ) {
				if( Get( out Transform3Module tm ) ) {
					tm.Transform.Rotation = Quaternion.FromAxisAngle( Vector3.UnitY, ( data.LastPositionLocked - data.PositionLocked ).X / (float) ( Math.PI * 2 * 100 ) ) * tm.Transform.Rotation;
					tm.Transform.Rotation = tm.Transform.Rotation.Normalized;
					cameraTransform.Rotation = Quaternion.FromAxisAngle( cameraTransform.Rotation.Right, ( data.LastPositionLocked - data.PositionLocked ).Y / (float) ( Math.PI * 2 * 100 ) ) * cameraTransform.Rotation;
					cameraTransform.Rotation = cameraTransform.Rotation.Normalized;
				}
			} else {
				Camera.TranformInterface.Rotation = Quaternion.FromAxisAngle( Vector3.UnitY, ( data.LastPositionLocked - data.PositionLocked ).X / (float) ( Math.PI * 2 * 100 ) ) * Camera.TranformInterface.Rotation;
				Camera.TranformInterface.Rotation = Camera.TranformInterface.Rotation.Normalized;
				Camera.TranformInterface.Rotation = Quaternion.FromAxisAngle( Camera.TranformInterface.Rotation.Right, ( data.LastPositionLocked - data.PositionLocked ).Y / (float) ( Math.PI * 2 * 100 ) ) * Camera.TranformInterface.Rotation;
				Camera.TranformInterface.Rotation = Camera.TranformInterface.Rotation.Normalized;
			}
		}

		public void WheelScrollChangeHandler( IntPtr window, float delta, MouseInputEventData data ) {

		}

	}
}
