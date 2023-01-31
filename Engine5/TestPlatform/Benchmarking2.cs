using BenchmarkDotNet.Attributes;
using Engine;
using Engine.Datatypes.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestPlatform;

[MemoryDiagnoser]
public unsafe class Benchmarking2 {


	[Params( 8192 )]
	public int Size { get; set; }

	//public void* Source;
	//public void* Destination;
	public SortedSet<int> _set;
	public BinaryTree<int> _binaryTree;
	public int[] _toInsert;

	[GlobalSetup]
	public void Setup() {
		Random rand = new();
		//Source = NativeMemory.Alloc( (nuint) ( Size * sizeof( int ) ) );
		//Destination = NativeMemory.Alloc( (nuint) ( Size * sizeof( int ) ) );
		//for ( ulong i = 0; i < Size; i++ ) {
		//	( (int*) Source )[ i ] = rand.Next();
		//}
		_set = new();
		_binaryTree = new();
		_toInsert = new int[ Size ];
		for ( int i = 0; i < Size; i++ ) {
			_toInsert[ i ] = rand.Next();
		}
	}


	[Benchmark]
	public void Set() {
		_set.Clear();
		for ( int i = 0; i < Size; i++ )
			_set.Add( _toInsert[ i ] );
	}


	[Benchmark]
	public void Tree() {
		_binaryTree.Clear();
		for ( int i = 0; i < Size; i++ )
			_binaryTree.Add( _toInsert[ i ] );
	}


	//[Benchmark]
	//public void MemoryCopy() {
	//	Buffer.MemoryCopy( Source, Destination, Size * sizeof( int ), Size * sizeof( int ) );
	//}

	//[Benchmark]
	//public void CopyBlock() {
	//	Unsafe.CopyBlock( Source, Destination, (uint) ( Size * sizeof( int ) ) );
	//}


	//[Benchmark]
	//public void MemoryCopyQuart() {
	//	Buffer.MemoryCopy( Source, Destination, Size * sizeof( int ), Size * sizeof( int ) / 4 );
	//}

	//[Benchmark]
	//public void MemoryCopyQuart2() {
	//	Buffer.MemoryCopy( Source, Destination, Size * sizeof( int ) / 4, Size * sizeof( int ) / 4 );
	//}

	//[Benchmark]
	//public void CopyBlockQuart() {
	//	Unsafe.CopyBlock( Source, Destination, (uint) ( Size * sizeof( int ) / 4 ) );
	//}

	//[Benchmark]
	//public void DoubleToUlong() {
	//	5.43.Convert<double, ulong>();
	//}

	//[Benchmark]
	//public void UintToLong() {
	//	65034u.Convert<uint, long>();
	//}

	//[Benchmark]
	//public void UlongToUlong() {
	//	650343424267716ul.Convert<ulong, ulong>();
	//}

	//[Benchmark]
	//public void UlongToUint() {
	//	650343424267716ul.Convert<ulong, uint>();
	//}

	//[Benchmark]
	//public void DoubleToUlong2() {
	//	5.43.Convert2<double, ulong>();
	//}

	//[Benchmark]
	//public void UintToLong2() {
	//	65034u.Convert2<uint, long>();
	//}

	//[Benchmark]
	//public void UlongToUlong2() {
	//	650343424267716ul.Convert2<ulong, ulong>();
	//}

	//[Benchmark]
	//public void UlongToUint2() {
	//	650343424267716ul.Convert2<ulong, uint>();
	//}

	//[Benchmark]
	//public void Nothing() {

	//}

}
