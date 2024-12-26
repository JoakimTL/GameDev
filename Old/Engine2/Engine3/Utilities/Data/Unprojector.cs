using Engine.Graphics.Objects;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Utilities.Data {
	public static class Unprojector {
		public static Vector3 GetMouseUnprojected( IView view, GLWindow window, InputEventHandler eventHandler ) {
			Vector2 mouseNDC = new Vector2( eventHandler.Mouse.Data.Position.X / window.Size.X * 2 - 1, 1 - eventHandler.Mouse.Data.Position.Y / window.Size.Y * 2 );
			Vector4 mouseVector = new Vector4( mouseNDC.X, mouseNDC.Y, -1, 1 );
			Vector4 mouseEye = mouseVector * view.IPMatrix;
			mouseEye.Z = -1;
			mouseEye.W = 0;
			Vector4 mouseWorld = mouseEye * view.IVMatrix;

			return Vector3.Normalize( mouseWorld.XYZ );
		}

		public static Vector3 GetMouseUnprojected( IView view, Vector2 ndc ) {
			Vector4 mouseVector = new Vector4( ndc.X, ndc.Y, -1, 1 );
			Vector4 mouseEye = mouseVector * view.IPMatrix;
			mouseEye.Z = -1;
			mouseEye.W = 0;
			Vector4 mouseWorld = mouseEye * view.IVMatrix;

			return Vector3.Normalize( mouseWorld.XYZ );
		}

		public static Vector3 GetMouseUnprojected( Matrix4 inverseProjection, Matrix4 inverseView, Vector2 ndc ) {
			Vector4 mouseVector = new Vector4( ndc.X, ndc.Y, -1, 1 );
			Vector4 mouseEye = mouseVector * inverseProjection;
			mouseEye.Z = -1;
			mouseEye.W = 0;
			Vector4 mouseWorld = mouseEye * inverseView;

			return Vector3.Normalize( mouseWorld.XYZ );
		}
	}
}
