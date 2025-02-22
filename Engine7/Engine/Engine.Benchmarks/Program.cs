using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Engine.Benchmarks;

//Output results to a file
//BenchmarkRunner.Run<EventsVsAbstractMethod>();
//BenchmarkRunner.Run<DictionaryKeyAddingTests>();
//BenchmarkRunner.Run<CodeMark>();
BenchmarkRunner.Run( [ typeof( DictionaryKeyAddingTests ), typeof( DictionaryKeyGetterTests ) ], ManualConfig.Create( DefaultConfig.Instance ).WithOptions( ConfigOptions.JoinSummary ).KeepBenchmarkFiles() );
//BenchmarkRunner.Run( typeof( Program ).Assembly, ManualConfig.Create( DefaultConfig.Instance ).WithOptions( ConfigOptions.JoinSummary ).KeepBenchmarkFiles() );

Console.WriteLine( "Press any key to exit..." );
Console.ReadKey();

/*
 * NET 9.0
| Type                     | Method                | Mean      | Error    | StdDev   | Allocated |
|------------------------- |---------------------- |----------:|---------:|---------:|----------:|
| DictionaryKeyAddingTests | AddRemoveTestInt      |  77.62 ns | 0.397 ns | 0.371 ns |         - |
| DictionaryKeyGetterTests | GetTestInt            |  37.02 ns | 0.527 ns | 0.493 ns |         - |
| DictionaryKeyAddingTests | AddRemoveTestString   | 137.63 ns | 0.908 ns | 0.849 ns |         - |
| DictionaryKeyGetterTests | GetTestString         |  74.06 ns | 1.165 ns | 1.032 ns |         - |
| DictionaryKeyAddingTests | AddRemoveTestType     | 148.72 ns | 1.816 ns | 1.698 ns |         - |
| DictionaryKeyGetterTests | GetTestType           |  82.55 ns | 0.949 ns | 0.842 ns |         - |
| DictionaryKeyAddingTests | AddRemoveTestGuid     | 109.83 ns | 0.999 ns | 0.886 ns |         - |
| DictionaryKeyGetterTests | GetTestGuid           |  47.14 ns | 0.584 ns | 0.546 ns |         - |

| Type                     | Method                | Mean      | Error    | StdDev   | Gen0   | Allocated |
|------------------------- |---------------------- |----------:|---------:|---------:|-------:|----------:|
| DictionaryKeyAddingTests | AddRemoveTestInt      |  81.37 ns | 0.677 ns | 0.600 ns |      - |         - |
| DictionaryKeyGetterTests | GetTestInt            |  36.48 ns | 0.378 ns | 0.335 ns |      - |         - |
| DictionaryKeyAddingTests | AddRemoveTestLong     |  76.99 ns | 0.383 ns | 0.340 ns |      - |         - |
| DictionaryKeyGetterTests | GetTestLong           |  37.02 ns | 0.425 ns | 0.398 ns |      - |         - |
| DictionaryKeyAddingTests | AddRemoveTestLongLong | 132.12 ns | 0.720 ns | 0.638 ns | 0.0062 |     320 B |
| DictionaryKeyGetterTests | GetTestLongLong       |  68.79 ns | 0.245 ns | 0.205 ns | 0.0063 |     320 B |
| DictionaryKeyAddingTests | AddRemoveTestString   | 134.95 ns | 0.883 ns | 0.826 ns |      - |         - |
| DictionaryKeyGetterTests | GetTestString         |  72.27 ns | 0.157 ns | 0.122 ns |      - |         - |
| DictionaryKeyAddingTests | AddRemoveTestType     | 144.42 ns | 1.047 ns | 0.874 ns |      - |         - |
| DictionaryKeyGetterTests | GetTestType           |  82.87 ns | 1.001 ns | 0.937 ns |      - |         - |
| DictionaryKeyAddingTests | AddRemoveTestGuid     | 106.99 ns | 0.810 ns | 0.757 ns |      - |         - |
| DictionaryKeyGetterTests | GetTestGuid           |  47.38 ns | 0.758 ns | 0.709 ns |      - |         - |
 * 
 * 
 * 
 */