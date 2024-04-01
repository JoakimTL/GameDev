
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
//using Engine.Math.Benchmarks;

//T negate<T>(T v) where T : INumberBase<T> => -v;

//uint a = 6;
//uint b = negate( a );

//Console.WriteLine(b);

//Console.WriteLine( System.Numerics.Matrix4x4.CreatePerspectiveFieldOfView( 90F / 180 * MathF.PI, 1, 0.1F, 100F ) );

BenchmarkRunner.Run( typeof( Program ).Assembly, // all benchmarks from given assembly are going to be executed
    ManualConfig.Create( DefaultConfig.Instance ).WithOptions( ConfigOptions.JoinSummary | ConfigOptions.DisableLogFile ) );

Console.ReadLine();

/*
 * Results:
| Type               | Method           | Mean      | Error     | StdDev    | Median    |
|------------------- |----------------- |----------:|----------:|----------:|----------:|
| NonSIMDBenchmarks  | Vector2Add       | 5.5351 ns | 0.0621 ns | 0.0581 ns | 5.5498 ns |
| NumericsBenchmarks | Vector2Add       | 0.2153 ns | 0.0135 ns | 0.0126 ns | 0.2139 ns |
| NonSIMDBenchmarks  | Vector3Add       | 0.2258 ns | 0.0167 ns | 0.0156 ns | 0.2207 ns |
| NumericsBenchmarks | Vector3Add       | 0.1817 ns | 0.0109 ns | 0.0102 ns | 0.1804 ns |
| NonSIMDBenchmarks  | Vector4Add       | 0.2931 ns | 0.0087 ns | 0.0081 ns | 0.2940 ns |
| NumericsBenchmarks | Vector4Add       | 0.2498 ns | 0.0124 ns | 0.0116 ns | 0.2484 ns |
| NonSIMDBenchmarks  | Vector2Subtract  | 5.5570 ns | 0.0772 ns | 0.0722 ns | 5.5788 ns |
| NumericsBenchmarks | Vector2Subtract  | 0.2162 ns | 0.0155 ns | 0.0145 ns | 0.2120 ns |
| NonSIMDBenchmarks  | Vector3Subtract  | 0.2576 ns | 0.0158 ns | 0.0148 ns | 0.2551 ns |
| NumericsBenchmarks | Vector3Subtract  | 0.1871 ns | 0.0123 ns | 0.0109 ns | 0.1861 ns |
| NonSIMDBenchmarks  | Vector4Subtract  | 0.2788 ns | 0.0156 ns | 0.0146 ns | 0.2793 ns |
| NumericsBenchmarks | Vector4Subtract  | 0.2571 ns | 0.0100 ns | 0.0088 ns | 0.2583 ns |
| NonSIMDBenchmarks  | Vector2Multiply  | 5.4090 ns | 0.0789 ns | 0.0738 ns | 5.4178 ns |
| NumericsBenchmarks | Vector2Multiply  | 0.2306 ns | 0.0088 ns | 0.0078 ns | 0.2331 ns |
| NonSIMDBenchmarks  | Vector3Multiply  | 0.2329 ns | 0.0091 ns | 0.0085 ns | 0.2340 ns |
| NumericsBenchmarks | Vector3Multiply  | 0.2119 ns | 0.0120 ns | 0.0113 ns | 0.2130 ns |
| NonSIMDBenchmarks  | Vector4Multiply  | 0.2679 ns | 0.0100 ns | 0.0094 ns | 0.2688 ns |
| NumericsBenchmarks | Vector4Multiply  | 0.2593 ns | 0.0154 ns | 0.0144 ns | 0.2596 ns |
| NonSIMDBenchmarks  | Vector2Divide    | 5.4116 ns | 0.0407 ns | 0.0381 ns | 5.4117 ns |
| NumericsBenchmarks | Vector2Divide    | 0.2173 ns | 0.0108 ns | 0.0096 ns | 0.2158 ns |
| NonSIMDBenchmarks  | Vector3Divide    | 0.2195 ns | 0.0118 ns | 0.0105 ns | 0.2194 ns |
| NumericsBenchmarks | Vector3Divide    | 0.2008 ns | 0.0118 ns | 0.0105 ns | 0.2010 ns |
| NonSIMDBenchmarks  | Vector4Divide    | 0.2842 ns | 0.0129 ns | 0.0121 ns | 0.2846 ns |
| NumericsBenchmarks | Vector4Divide    | 0.2839 ns | 0.0094 ns | 0.0088 ns | 0.2833 ns |
| NonSIMDBenchmarks  | Vector2Dot       | 0.0173 ns | 0.0107 ns | 0.0100 ns | 0.0159 ns |
| NumericsBenchmarks | Vector2Dot       | 0.6005 ns | 0.0107 ns | 0.0095 ns | 0.6033 ns |
| NonSIMDBenchmarks  | Vector3Dot       | 0.0058 ns | 0.0076 ns | 0.0071 ns | 0.0045 ns |
| NumericsBenchmarks | Vector3Dot       | 0.6202 ns | 0.0176 ns | 0.0165 ns | 0.6181 ns |
| NonSIMDBenchmarks  | Vector4Dot       | 0.0016 ns | 0.0036 ns | 0.0034 ns | 0.0000 ns |
| NumericsBenchmarks | Vector4Dot       | 0.6121 ns | 0.0123 ns | 0.0115 ns | 0.6111 ns |
| NonSIMDBenchmarks  | Vector2Normalize | 5.4858 ns | 0.0536 ns | 0.0502 ns | 5.4937 ns |
| NumericsBenchmarks | Vector2Normalize | 0.7473 ns | 0.0208 ns | 0.0194 ns | 0.7479 ns |
| NonSIMDBenchmarks  | Vector3Normalize | 0.0536 ns | 0.0113 ns | 0.0106 ns | 0.0542 ns |
| NumericsBenchmarks | Vector3Normalize | 0.8590 ns | 0.0291 ns | 0.0272 ns | 0.8655 ns |
| NonSIMDBenchmarks  | Vector4Normalize | 0.0554 ns | 0.0109 ns | 0.0102 ns | 0.0567 ns |
| NumericsBenchmarks | Vector4Normalize | 0.8680 ns | 0.0295 ns | 0.0276 ns | 0.8687 ns |
| NonSIMDBenchmarks  | Vector3Cross     | 0.2733 ns | 0.0086 ns | 0.0080 ns | 0.2733 ns |
| NumericsBenchmarks | Vector3Cross     | 0.2976 ns | 0.0154 ns | 0.0136 ns | 0.2996 ns |

| Method           | Mean      | Error     | StdDev    | Median    |
|----------------- |----------:|----------:|----------:|----------:|
| Vector2Add       | 0.0537 ns | 0.0078 ns | 0.0073 ns | 0.0537 ns |
| Vector3Add       | 0.0667 ns | 0.0164 ns | 0.0153 ns | 0.0701 ns |
| Vector4Add       | 0.2936 ns | 0.0087 ns | 0.0081 ns | 0.2935 ns |
| Vector2Subtract  | 0.0280 ns | 0.0147 ns | 0.0138 ns | 0.0273 ns |
| Vector3Subtract  | 0.0953 ns | 0.0121 ns | 0.0114 ns | 0.0962 ns |
| Vector4Subtract  | 0.1103 ns | 0.0116 ns | 0.0108 ns | 0.1042 ns |
| Vector2Multiply  | 0.0631 ns | 0.0138 ns | 0.0129 ns | 0.0657 ns |
| Vector3Multiply  | 0.0697 ns | 0.0124 ns | 0.0116 ns | 0.0692 ns |
| Vector4Multiply  | 0.3015 ns | 0.0151 ns | 0.0142 ns | 0.2957 ns |
| Vector2Divide    | 0.0623 ns | 0.0205 ns | 0.0192 ns | 0.0658 ns |
| Vector3Divide    | 0.0505 ns | 0.0088 ns | 0.0082 ns | 0.0469 ns |
| Vector4Divide    | 0.2806 ns | 0.0128 ns | 0.0120 ns | 0.2774 ns |
| Vector2Dot       | 0.0021 ns | 0.0032 ns | 0.0028 ns | 0.0000 ns |
| Vector3Dot       | 0.0051 ns | 0.0100 ns | 0.0094 ns | 0.0000 ns |
| Vector4Dot       | 0.0010 ns | 0.0032 ns | 0.0028 ns | 0.0000 ns |
| Vector2Normalize | 0.2805 ns | 0.0152 ns | 0.0142 ns | 0.2802 ns |
| Vector3Normalize | 0.2504 ns | 0.0121 ns | 0.0114 ns | 0.2490 ns |
| Vector4Normalize | 0.2880 ns | 0.0145 ns | 0.0135 ns | 0.2893 ns |
| Vector3Cross     | 0.2524 ns | 0.0152 ns | 0.0142 ns | 0.2530 ns |
 * 
 * 
 */