using Engine.Data;

namespace Engine.Tests.Data;

[TestFixture]
public sealed class UnmanagedListTests {

	[Test]
	public void Add() {
		UnmanagedList list = new();
		list.Add( 1 );
		list.Add( 2 );
		list.Add( 3 );
		Assert.That( list.Read<int>( 0 ), Is.EqualTo( 1 ) );
		Assert.That( list.Read<int>( 4 ), Is.EqualTo( 2 ) );
		Assert.That( list.Read<int>( 8 ), Is.EqualTo( 3 ) );
	}

	[Test]
	public void AddRange() {
		UnmanagedList list = new();
		list.AddRange( stackalloc []{ 1, 2, 3 } );
		Assert.That( list.Read<int>( 0 ), Is.EqualTo( 1 ) );
		Assert.That( list.Read<int>( 4 ), Is.EqualTo( 2 ) );
		Assert.That( list.Read<int>( 8 ), Is.EqualTo( 3 ) );
	}

	[Test]
	public void Set() {
		UnmanagedList list = new();
		list.Add( 1 );
		list.Add( 2 );
		list.Add( 3 );
		list.Set( 4, 4 );
		Assert.That( list.Read<int>( 4 ), Is.EqualTo( 4 ) );
	}

	[Test]
	public void SetRange() {
		UnmanagedList list = new();
		list.AddRange( stackalloc[] { 1, 2, 3 } );
		list.Set( stackalloc[] { 4, 5 }, 4 );
		Assert.That( list.Read<int>( 4 ), Is.EqualTo( 4 ) );
		Assert.That( list.Read<int>( 8 ), Is.EqualTo( 5 ) );
	}

	[Test]
	public void Dispose() {
		UnmanagedList list = new();
		list.Add( 1 );
		list.Dispose();
		Assert.That( list.Disposed, Is.True );
	}

	[Test]
	public void ResizeToFit() {
		UnmanagedList list = new( 16 );
		list.Add( 1 );
		list.Add( 2 );
		list.Add( 3 );
		list.Add( 4 );
		Assert.That( list.SizeBytes, Is.EqualTo( (nuint) 16u ) );
		list.Add( 5 );
		Assert.That( list.SizeBytes, Is.EqualTo( (nuint) 32u ) );
	}

	[Test]
	public void Read() {
		UnmanagedList list = new();
		list.Add( 1 );
		list.Add( 2 );
		list.Add( 3 );
		Assert.That( list.Read<int>( 0 ), Is.EqualTo( 1 ) );
		Assert.That( list.Read<int>( 4 ), Is.EqualTo( 2 ) );
		Assert.That( list.Read<int>( 8 ), Is.EqualTo( 3 ) );
	}

	[Test]
	public void TryRead() {
		UnmanagedList list = new();
		list.Add( 1 );
		list.Add( 2 );
		list.Add( 3 );
		Span<int> result = stackalloc int[ 3 ];
		bool success = list.TryRead( 0, result );
		Assert.That( success, Is.True );
		Assert.That( result[ 0 ], Is.EqualTo( 1 ) );
		Assert.That( result[ 1 ], Is.EqualTo( 2 ) );
		Assert.That( result[ 2 ], Is.EqualTo( 3 ) );
	}

	[Test]
	public void ToByteArray() {
		UnmanagedList list = new();
		list.Add( 0xff00ff00 );
		byte[] arr = list.ToArray<byte>( 0, 4 );
		Assert.That( arr[ 0 ], Is.EqualTo( 0x00 ) );
		Assert.That( arr[ 1 ], Is.EqualTo( 0xff ) );
		Assert.That( arr[ 2 ], Is.EqualTo( 0x00 ) );
		Assert.That( arr[ 3 ], Is.EqualTo( 0xff ) );
	}

}
