using Engine;
using Engine.Modularity.Modules;
using Engine.Modularity.Modules.Submodules;
using Engine.Rendering.Standard.UI.Standard.Text;
using TestPlatform;

//int xn = 32;
//int yn = 32;
//int zn = 32;
//double f = 0;
//long n = 0;
//for ( int x = 0; x < xn; x++ ) {
//	for ( int y = 0; y < yn; y++ ) {
//		for ( int z = 0; z < zn; z++ ) {
//			float v = DepthFieldMapGenerator.GetDataValue( new Engine.Data.Datatypes.Vector3i( x, y, z ) );
//			f += v;
//			n++;
//			//Console.WriteLine( $"{x},{y},{z}:{v }" );
//		}
//	}
//}
//Console.WriteLine( "NUM: " + n );
//Console.WriteLine( "AVG: " + ( f / n ) );

//double avg = f / n;
//double variance = 0;
//for ( int x = 0; x < xn; x++ ) {
//	for ( int y = 0; y < yn; y++ ) {
//		for ( int z = 0; z < zn; z++ ) {
//			float v = DepthFieldMapGenerator.GetDataValue( new Engine.Data.Datatypes.Vector3i( x, y, z ) );
//			variance += ( v - avg ) * ( v - avg );
//			//Console.WriteLine( $"{x},{y},{z}:{v }" );
//		}
//	}
//}
//variance /= n - 1;
//Console.WriteLine( "VAR: " + variance );
//Console.WriteLine( "SD:  " + Math.Sqrt( variance ) );
/*
Random r1 = new Random( 55 );
byte[][] segments = new byte[ 6 ][];
segments[ 0 ] = new byte[ 32 ];
segments[ 1 ] = new byte[ 64 ];
segments[ 2 ] = new byte[ 128 ];
segments[ 3 ] = new byte[ 512 ];
segments[ 4 ] = new byte[ 2048 ];
segments[ 5 ] = new byte[ 65536 ];
unchecked {
	for ( int j = 0; j < segments.Length; j++ )
		for ( int i = 0; i < segments[ j ].Length; i++ )
			segments[ j ][ i ] = (byte) r1.Next();
}

byte[]? o = Engine.Data.Segmentation.Segment( segments );
byte[][]? p = Engine.Data.Segmentation.Parse( o );
*/
/*
Console.WriteLine( "out" );
for ( int i = 0; i < o.Length; i++ ) {
	Console.Write( o[ i ].ToString( "X2" ) );
	if ( i % 128 == 0 ) {
		Console.WriteLine();
	} else if ( i % 8 == 0 ) {
		Console.Write( " " );
	} else if ( i % 4 == 0 )
		Console.Write( "_" );
}
Console.WriteLine( "in" );
for ( int i = 0; i < p.Length; i++ ) {
	for ( int j = 0; j < p[ i ].Length; j++ ) {
		Console.Write( p[ i ][ j ].ToString( "X2" ) );
		if ( j % 128 == 0 ) {
			Console.WriteLine();
		} else if ( j % 8 == 0 ) {
			Console.Write( " " );
		} else if ( j % 4 == 0 )
			Console.Write( "_" );
	}
	Console.WriteLine();
}
*/
/*
if ( p.Length == segments.Length )
	Console.WriteLine( "Success, same amount of arrays!" );

for ( int j = 0; j < p.Length; j++ )
	for ( int i = 0; i < p[ j ].Length; i++ ) {
		if ( p[ j ][ i ] != segments[ j ][ i ] )
			Console.WriteLine( $"Failed at [{j}][{i}]!" );
	}
*//*
int h = 0;
double avg = 0;

int j = 100023;
Guid guid = Guid.NewGuid();
double time = Engine.Time.Clock64.StartupTime;
for ( int i = 0; i < 10_000_000; i++ ) {
	unchecked {
		h += Guid.NewGuid().GetHashCode();
	}
}
double time2 = Engine.Time.Clock64.StartupTime;
avg = time2 - time;
Console.WriteLine( $"{( avg ) * 1000}ms" );
Console.WriteLine( h );*/
/*
Entity e = new( "Bærnt" );
Transform3Component? t = e.AddComponent<Transform3Component>();
t.Transform.Translation = new System.Numerics.Vector3( 33, -15, -777 );
File.WriteAllBytes( "Bærnt.txt", Entity.Save( e ) ?? Array.Empty<byte>() );
*/


/*Entity? test = Entity.Load( File.ReadAllBytes( "Bærnt.txt" ) );
Console.WriteLine( test );
Console.WriteLine( test.TryGetComponent<Transform3Component>( out _ ) );
Console.WriteLine( test.GetComponent<Transform3Component>().Transform.Translation );*/
/*
Server server = new Server();
Client client = new Client();
Thread.Sleep( 4000 );
Console.WriteLine( $"Server received {server.received} packets" );
Console.WriteLine( $"Client send {client.bytesSent} bytes, while the server received {server.bytesReceived} bytes!" );
*/
/*
BitArray ba = new BitArray( 511 );
ba.SetAll( true );
byte[] baa = new byte[ (int) Math.Ceiling( (double) ba.Count / ( sizeof( byte ) * 8 ) ) ];
ba.CopyTo( baa, 0 );
BitArray bb = new BitArray( baa );

Console.WriteLine( string.Join( ", ", baa ) );

for ( int i = 0; i < bb.Length; i++ ) {
	Console.WriteLine(bb[i]);
}
*/

/*
Startup.Start(
	ModuleGenerator.CreateModule( "Client", 0, true,
	ModuleGenerator.GetRenderSubmodules(),
	ModuleGenerator.GetBasicSubmodules(),
	ModuleGenerator.GetClientSubmodules(),
	new[] { typeof( Render2Submodule ), typeof( Render3Submodule ), typeof( EntityManagementTestSubmodule ) } ),

	ModuleGenerator.CreateModule( "Server", 1, false, 50,
	ModuleGenerator.GetBasicSubmodules(),
	ModuleGenerator.GetServerSubmodules(),
	new[] { typeof( EntityNetworkManagementSubmodule ) } )
);
*/

/*
VoxelChunkBase c = new RenderVoxelChunk( 0, 1 );
Console.WriteLine( 2 * VoxelChunkBase.Area / 2 );
Console.WriteLine( 2 * VoxelChunkBase.Length / 2 );
Console.WriteLine( 2 / 2 );
Console.WriteLine( 2 * VoxelChunkBase.Area / 2 + 2 * VoxelChunkBase.Length / 2 + 2 / 2 );
Console.WriteLine();
Console.WriteLine( c.GetLocalVoxelIndex( (2, 2, 2) ) );
for ( int y = 0; y < VoxelChunkBase.Length; y++ ) {
	for ( int z = 0; z < VoxelChunkBase.Length; z++ ) {
		for ( int x = 0; x < VoxelChunkBase.Length; x++ ) {
			Console.WriteLine( $"{x}, {y}, {z}: {c.GetIdFromLocalPosition( (x, y, z) )}" );
		}
	}
}
*/

/*
Startup.Start(
	ModuleGenerator.CreateModule( "Client", 0, true,
	ModuleGenerator.GetRenderSubmodules(),
	ModuleGenerator.GetBasicSubmodules(),
	//ModuleGenerator.GetClientSubmodules(),
	new[] { typeof( Render2Submodule ), typeof( Render3Submodule ), typeof( TestRenderSubmodule ) } )
);
*/

Startup.Start(
	ModuleGenerator.CreateModule(
		"Test", 0, true,
		ModuleGenerator.GetRenderSubmodules(),
		new[] { typeof(RenderSubmodule), typeof(VoxelTestSubmodule)}
	)
);

/*
Console.WriteLine( new string( '_', 100 ) );

string text = "this is a test\nobviously it's not going to work properly at the moment, but we'll get it fixed!\nthe quick brown fox jumps over the lazy dog. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat. ";
TextContainer tc = new( new Font( "res/textures/fonts/calibri" ), text, 0.01f, true, false, wordWrapMode: WordWrapMode.WrapWords );
tc.Update();
Console.WriteLine( string.Join( '\n', tc.Lines.Select( p => p.ToString() ) ) );

Console.WriteLine( new string( '_', 100 ) );
*/

/*
double avg = 0;
for ( int i = 0; i < 100; i++ ) {
	double price = 30000;
	double stocks = 0;
	double investment = 0;
	Random r = new();
	for ( int y = 0; y < 30; y++ ) {
		for ( int m = 0; m < 12; m++ ) {
			double monthInvest = 1000;
			stocks += monthInvest / price;
			investment += monthInvest;
			price *= 1 + ( .1 + ( r.NextDouble() * 0.25 - 0.125 ) ) / 12;
		}
	}

	Console.WriteLine( stocks );
	Console.WriteLine( investment );
	Console.WriteLine( stocks * price );
	Console.WriteLine( stocks * price - investment );
	Console.WriteLine( ( stocks * price - investment ) * ( 1 - 0.34 ) );

	avg += ( stocks * price - investment ) * ( 1 - 0.34 ) + investment;
}

Console.WriteLine( $"Avg return + investment: {avg / 100}" );
*/
