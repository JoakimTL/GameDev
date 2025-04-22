using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine;
using OpenGL;
using System.Runtime.InteropServices;

namespace Civs.Render.World.Lines;

[Identity( nameof( Line3SceneData ) )]
[VAO.Setup( 0, 1, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public readonly struct Line3SceneData {
	[VAO.Data( VertexAttribType.Float, 4 ), FieldOffset( 0 )]
	public readonly Vector4<float> PointA;
	[VAO.Data( VertexAttribType.Float, 4 ), FieldOffset( 16 )]
	public readonly Vector4<float> PointB;
	[VAO.Data( VertexAttribType.Float, 3 ), FieldOffset( 32 )]
	public readonly Vector3<float> LineNormal;
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 44 )]
	public readonly Vector2<float> FillAnchors;
	[VAO.Data( VertexAttribType.Float, 4 ), FieldOffset( 52 )]
	public readonly Vector4<float> FillQuadratic;
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 68 )]
	public readonly Vector2<float> DistanceGradient;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 76 )]
	public readonly Vector4<byte> ColorStart;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 80 )]
	public readonly Vector4<byte> ColorEnd;

	public Line3SceneData( Vector3<float> pointA, float widthA, Vector3<float> pointB, float widthB, Vector3<float> lineNormal, float negativeAnchor, float positiveAnchor, Vector3<float> quadratic, float gradientWidth, float invisibleAtDistance, float distanceGradient, Vector4<byte> color ) {
		this.PointA = new( pointA.X, pointA.Y, pointA.Z, widthA );
		this.PointB = new( pointB.X, pointB.Y, pointB.Z, widthB );
		this.LineNormal = lineNormal;
		this.FillAnchors = new( negativeAnchor, positiveAnchor );
		this.FillQuadratic = new( quadratic.X, quadratic.Y, quadratic.Z, gradientWidth );
		this.DistanceGradient = new( invisibleAtDistance, distanceGradient );
		this.ColorStart = color;
		this.ColorEnd = color;
	}

	public Line3SceneData( Vector3<float> pointA, float widthA, Vector3<float> pointB, float widthB, Vector3<float> lineNormal, float negativeAnchor, float positiveAnchor, Vector3<float> quadratic, float gradientWidth, float invisibleAtDistance, float distanceGradient, Vector4<byte> colorStart, Vector4<byte> colorEnd ) {
		this.PointA = new( pointA.X, pointA.Y, pointA.Z, widthA );
		this.PointB = new( pointB.X, pointB.Y, pointB.Z, widthB );
		this.LineNormal = lineNormal;
		this.FillAnchors = new( negativeAnchor, positiveAnchor );
		this.FillQuadratic = new( quadratic.X, quadratic.Y, quadratic.Z, gradientWidth );
		this.DistanceGradient = new( invisibleAtDistance, distanceGradient );
		this.ColorStart = colorStart;
		this.ColorEnd = colorEnd;
	}
}
