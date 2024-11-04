//This might be very confusing at first. This entire project contains code used in Engine and above, except the Main function. Here we write to the log files from the log pipe in the LogLogic.

using System.IO.Pipes;

if (args.Length <= 0)
	throw new ArgumentException( "No name given!" );
string pipeName = args[ 0 ];
var dateTimeNowString = DateTime.Now.ToString( "yyyyMMdd.HHmmssff" );
string path = $"logs/normal/{dateTimeNowString}.log";
NamedPipeClientStream pipeClient = new( ".", pipeName, PipeDirection.In );

try {
	pipeClient.Connect( 5000 );
} catch (Exception e) {
	LogString( path, "Failed to connect to pipe" );
	LogString( path, e.ToString() );
	return;
}
byte[] data = new byte[ ushort.MaxValue + 1 ];

while (pipeClient.IsConnected || pipeClient.Length > 0) {
	try {
		int read = pipeClient.Read( data, 0, data.Length );
		unsafe {
			fixed (byte* ptr = data) {
				char* charPtr = (char*) ptr;
				var logString = new string( charPtr, 0, read / sizeof( char ) );
				LogString( path, logString );
			}
		}
	} catch (Exception e) {
		Console.WriteLine( e );
	}
}
pipeClient.Close();
pipeClient.Dispose();
Console.WriteLine( "Disconnected Logging" );


static void LogString( string path, string content ) {
	while (true) {
		try {
			string? directory = Path.GetDirectoryName( path );
			if (directory is null)
				return;
			if (!Directory.Exists( directory ))
				Directory.CreateDirectory( directory );
			File.AppendAllText( path, content );
			return;
		} catch (Exception e) {
			Console.WriteLine( e );
		}
	}
}