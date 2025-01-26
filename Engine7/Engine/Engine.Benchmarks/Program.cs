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
| Type                     | Method              | Mean      | Error    | StdDev   | Allocated |
|------------------------- |-------------------- |----------:|---------:|---------:|----------:|
| DictionaryKeyAddingTests | AddRemoveTestInt    |  77.62 ns | 0.397 ns | 0.371 ns |         - |
| DictionaryKeyGetterTests | GetTestInt          |  37.02 ns | 0.527 ns | 0.493 ns |         - |
| DictionaryKeyAddingTests | AddRemoveTestString | 137.63 ns | 0.908 ns | 0.849 ns |         - |
| DictionaryKeyGetterTests | GetTestString       |  74.06 ns | 1.165 ns | 1.032 ns |         - |
| DictionaryKeyAddingTests | AddRemoveTestType   | 148.72 ns | 1.816 ns | 1.698 ns |         - |
| DictionaryKeyGetterTests | GetTestType         |  82.55 ns | 0.949 ns | 0.842 ns |         - |
| DictionaryKeyAddingTests | AddRemoveTestGuid   | 109.83 ns | 0.999 ns | 0.886 ns |         - |
| DictionaryKeyGetterTests | GetTestGuid         |  47.14 ns | 0.584 ns | 0.546 ns |         - |
 * 
 * 
 * 
 */