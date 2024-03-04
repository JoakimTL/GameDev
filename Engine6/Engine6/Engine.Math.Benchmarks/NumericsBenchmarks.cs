//using BenchmarkDotNet.Attributes;
//using System.Numerics;

//namespace Engine.Math.Benchmarks;

//public class NumericsBenchmarks {

//    [Benchmark]
//    public Vector2 Vector2Add() {
//        var a = new Vector2( 1, 2 );
//        var b = new Vector2( 3, 4 );
//        return a + b;
//    }

//    [Benchmark]
//    public Vector3 Vector3Add() {
//        var a = new Vector3( 1, 2, 3 );
//        var b = new Vector3( 4, 5, 6 );
//        return a + b;
//    }

//    [Benchmark]
//    public Vector4 Vector4Add() {
//        var a = new Vector4( 1, 2, 3, 4 );
//        var b = new Vector4( 5, 6, 7, 8 );
//        return a + b;
//    }

//    [Benchmark]
//    public Vector2 Vector2Subtract() {
//        var a = new Vector2( 1, 2 );
//        var b = new Vector2( 3, 4 );
//        return a - b;
//    }

//    [Benchmark]
//    public Vector3 Vector3Subtract() {
//        var a = new Vector3( 1, 2, 3 );
//        var b = new Vector3( 4, 5, 6 );
//        return a - b;
//    }

//    [Benchmark]
//    public Vector4 Vector4Subtract() {
//        var a = new Vector4( 1, 2, 3, 4 );
//        var b = new Vector4( 5, 6, 7, 8 );
//        return a - b;
//    }

//    [Benchmark]
//    public Vector2 Vector2Multiply() {
//        var a = new Vector2( 1, 2 );
//        var b = new Vector2( 3, 4 );
//        return a * b;
//    }

//    [Benchmark]
//    public Vector3 Vector3Multiply() {
//        var a = new Vector3( 1, 2, 3 );
//        var b = new Vector3( 4, 5, 6 );
//        return a * b;
//    }

//    [Benchmark]
//    public Vector4 Vector4Multiply() {
//        var a = new Vector4( 1, 2, 3, 4 );
//        var b = new Vector4( 5, 6, 7, 8 );
//        return a * b;
//    }

//    [Benchmark]
//    public Vector2 Vector2Divide() {
//        var a = new Vector2( 1, 2 );
//        var b = new Vector2( 3, 4 );
//        return a / b;
//    }

//    [Benchmark]
//    public Vector3 Vector3Divide() {
//        var a = new Vector3( 1, 2, 3 );
//        var b = new Vector3( 4, 5, 6 );
//        return a / b;
//    }

//    [Benchmark]
//    public Vector4 Vector4Divide() {
//        var a = new Vector4( 1, 2, 3, 4 );
//        var b = new Vector4( 5, 6, 7, 8 );
//        return a / b;
//    }

//    [Benchmark]
//    public float Vector2Dot() {
//        var a = new Vector2( 1, 2 );
//        var b = new Vector2( 3, 4 );
//        return Vector2.Dot( a, b );
//    }

//    [Benchmark]
//    public float Vector3Dot() {
//        var a = new Vector3( 1, 2, 3 );
//        var b = new Vector3( 4, 5, 6 );
//        return Vector3.Dot( a, b );
//    }

//    [Benchmark]
//    public float Vector4Dot() {
//        var a = new Vector4( 1, 2, 3, 4 );
//        var b = new Vector4( 5, 6, 7, 8 );
//        return Vector4.Dot( a, b );
//    }

//    [Benchmark]
//    public Vector2 Vector2Normalize() {
//        var a = new Vector2( 1, 2 );
//        return Vector2.Normalize( a );
//    }

//    [Benchmark]
//    public Vector3 Vector3Normalize() {
//        var a = new Vector3( 1, 2, 3 );
//        return Vector3.Normalize( a );
//    }

//    [Benchmark]
//    public Vector4 Vector4Normalize() {
//        var a = new Vector4( 1, 2, 3, 4 );
//        return Vector4.Normalize( a );
//    }

//    [Benchmark]
//    public Vector3 Vector3Cross() {
//        var a = new Vector3( 1, 2, 3 );
//        var b = new Vector3( 4, 5, 6 );
//        return Vector3.Cross( a, b );
//    }


//}