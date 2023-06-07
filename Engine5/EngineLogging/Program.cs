using System.IO.Pipes;

namespace EngineLogging;

public class Program {
	private const string _Warning = "WAR";
	private const string _Error = "ERR";

	public static void Main( string[] args ) {
		if ( args.Length <= 0 )
			throw new ArgumentException( "No name given!" );
		string pipeName = args[ 0 ];
		var dateTimeNowString = DateTime.Now.ToString( "yyyyMMdd_HH_mm_ss_fff" );
		string errorPath = $"logs/error/{dateTimeNowString}.log";
		string warningPath = $"logs/warning/{dateTimeNowString}.log";
		string normalPath = $"logs/normal/{dateTimeNowString}.log";
		NamedPipeClientStream pipeClient = new( ".", pipeName, PipeDirection.In );

		try {
			pipeClient.Connect( 5000 );
		} catch ( Exception e ) {
			LogString( errorPath, "Failed to connect to pipe" );
			LogString( errorPath, e.ToString() );
			return;
		}
		byte[] data = new byte[ ushort.MaxValue + 1 ];

		while ( pipeClient.IsConnected || pipeClient.Length > 0 ) {
			try {
				int read = pipeClient.Read( data, 0, data.Length );
				unsafe {
					fixed ( byte* ptr = data ) {
						char* charPtr = (char*) ptr;
						var logString = new string( charPtr, 0, read / sizeof( char ) );
						if ( logString.StartsWith( _Error ) ) {
							LogString( errorPath, logString );
						} else if ( logString.StartsWith( _Warning ) ) {
							LogString( warningPath, logString );
						} else {
							LogString( normalPath, logString );
						}
					}
				}
			} catch ( Exception e ) {
				Console.WriteLine( e );
			}
		}
		pipeClient.Close();
		pipeClient.Dispose();
		Console.WriteLine( "Disconnected Logging" );
	}

	public static void LogString( string path, string content ) {
		while ( true ) {
			try {
				string? directory = Path.GetDirectoryName( path );
				if ( directory is null )
					return;
				if ( !Directory.Exists( directory ) )
					Directory.CreateDirectory( directory );
				File.AppendAllText( path, content );
				return;
			} catch ( Exception e ) {
				Console.WriteLine( e );
			}
		}
	}
}