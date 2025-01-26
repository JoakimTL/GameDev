using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Engine.Benchmarks;

//Output results to a file
//BenchmarkRunner.Run<EventsVsAbstractMethod>();
//BenchmarkRunner.Run<DictionaryKeyAddingTests>();
BenchmarkRunner.Run<CodeMark>();
//BenchmarkRunner.Run( [ typeof( DictionaryKeyAddingTests ), typeof( DictionaryKeyGetterTests ) ], ManualConfig.Create( DefaultConfig.Instance ).WithOptions( ConfigOptions.JoinSummary ).KeepBenchmarkFiles() );
//BenchmarkRunner.Run( typeof( Program ).Assembly, ManualConfig.Create( DefaultConfig.Instance ).WithOptions( ConfigOptions.JoinSummary ).KeepBenchmarkFiles() );

Console.WriteLine( "Press any key to exit..." );
Console.ReadKey();