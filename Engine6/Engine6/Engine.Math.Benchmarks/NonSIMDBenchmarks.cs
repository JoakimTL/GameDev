//using BenchmarkDotNet.Attributes;

//namespace Engine.Math.Benchmarks;

//public class NonSIMDBenchmarks {

//    //Copy NumericsBenchmarks.cs here, but use Engine.Math instead of System.Numerics

//    [Benchmark]
//    public Vector2<float> Vector2Add() {
//        var a = new Vector2<float>( 1f, 2f );
//        var b = new Vector2<float>( 3f, 4f );
//        return a + b;
//    }

//    [Benchmark]
//    public Vector3<float> Vector3Add() {
//        var a = new Vector3<float>( 1f, 2f, 3f );
//        var b = new Vector3<float>( 4f, 5f, 6f );
//        return a + b;
//    }

//    [Benchmark]
//    public Vector4<float> Vector4Add() {
//        var a = new Vector4<float>( 1f, 2f, 3f, 4f );
//        var b = new Vector4<float>( 5f, 6f, 7f, 8f );
//        return a + b;
//    }

//    [Benchmark]
//    public Vector2<float> Vector2Subtract() {
//        var a = new Vector2<float>( 1f, 2f );
//        var b = new Vector2<float>( 3f, 4f );
//        return a - b;
//    }

//    [Benchmark]
//    public Vector3<float> Vector3Subtract() {
//        var a = new Vector3<float>( 1f, 2f, 3f );
//        var b = new Vector3<float>( 4f, 5f, 6f );
//        return a - b;
//    }

//    [Benchmark]
//    public Vector4<float> Vector4Subtract() {
//        var a = new Vector4<float>( 1f, 2f, 3f, 4f );
//        var b = new Vector4<float>( 5f, 6f, 7f, 8f );
//        return a - b;
//    }

//    [Benchmark]
//    public Vector2<float> Vector2Multiply() {
//        var a = new Vector2<float>( 1f, 2f );
//        var b = new Vector2<float>( 3f, 4f );
//        return a * b;
//    }

//    [Benchmark]
//    public Vector3<float> Vector3Multiply() {
//        var a = new Vector3<float>( 1f, 2f, 3f );
//        var b = new Vector3<float>( 4f, 5f, 6f );
//        return a * b;
//    }

//    [Benchmark]
//    public Vector4<float> Vector4Multiply() {
//        var a = new Vector4<float>( 1f, 2f, 3f, 4f );
//        var b = new Vector4<float>( 5f, 6f, 7f, 8f );
//        return a * b;
//    }

//    [Benchmark]
//    public Vector2<float> Vector2Divide() {
//        var a = new Vector2<float>( 1f, 2f );
//        var b = new Vector2<float>( 3f, 4f );
//        return a / b;
//    }

//    [Benchmark]
//    public Vector3<float> Vector3Divide() {
//        var a = new Vector3<float>( 1f, 2f, 3f );
//        var b = new Vector3<float>( 4f, 5f, 6f );
//        return a / b;
//    }

//    [Benchmark]
//    public Vector4<float> Vector4Divide() {
//        var a = new Vector4<float>( 1f, 2f, 3f, 4f );
//        var b = new Vector4<float>( 5f, 6f, 7f, 8f );
//        return a / b;
//    }

//    [Benchmark]
//    public float Vector2Dot() {
//        var a = new Vector2<float>( 1f, 2f );
//        var b = new Vector2<float>( 3f, 4f );
//        return a.Dot( b );
//    }

//    [Benchmark]
//    public float Vector3Dot() {
//        var a = new Vector3<float>( 1f, 2f, 3f );
//        var b = new Vector3<float>( 4f, 5f, 6f );
//        return a.Dot( b );
//    }

//    [Benchmark]
//    public float Vector4Dot() {
//        var a = new Vector4<float>( 1f, 2f, 3f, 4f );
//        var b = new Vector4<float>( 5f, 6f, 7f, 8f );
//        return a.Dot( b );
//    }

//    [Benchmark]
//    public Vector2<float> Vector2Normalize() {
//        var a = new Vector2<float>( 1f, 2f );
//        return a.Normalize();
//    }

//    [Benchmark]
//    public Vector3<float> Vector3Normalize() {
//        var a = new Vector3<float>( 1f, 2f, 3f );
//        return a.Normalize();
//    }

//    [Benchmark]
//    public Vector4<float> Vector4Normalize() {
//        var a = new Vector4<float>( 1f, 2f, 3f, 4f );
//        return a.Normalize();
//    }

//    [Benchmark]
//    public Vector3<float> Vector3Cross() {
//        var a = new Vector3<float>( 1f, 2f, 3f );
//        var b = new Vector3<float>( 4f, 5f, 6f );
//        return a.Cross( b );
//    }
//}
