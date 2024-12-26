using Engine.MemLib;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Utilities.IO {
	public class Log : Cacheable {

		private readonly string upperName;
		public readonly string DirectoryPath;
		public readonly string FilePath;
		public ConsoleColor DefaultColor { get; set; }
		public LogFlags DefaultFlags { get; set; }
		public bool Quiet = false;
		private Thread logThread;
		private BlockingCollection<string> newLines = new BlockingCollection<string>( 2048 );

		public Log( string name, ConsoleColor defaultColor = ConsoleColor.White ) : base( name ) {
			upperName = Name.ToUpper();
			DefaultColor = defaultColor;
			DefaultFlags = 0;
			string directorySafeName = Name.Replace( ' ', '_' );
			DirectoryPath = $"logs/{directorySafeName}/{Path.GetFileNameWithoutExtension( Process.GetCurrentProcess().MainModule.FileName )}";
			FilePath = $"logs/{directorySafeName}/{Path.GetFileNameWithoutExtension( Process.GetCurrentProcess().MainModule.FileName )}/{DateTime.Now.ToString( "yyyy_MM_dd_HH_mm_ss" )}.txt";
			new Task( StartLogging ).Start();
		}

		public void StartLogging() {
			Mem.InitializationDone.WaitOne();
			logThread = Mem.Threads.StartNew( this.LogFunc, $"[{Name}] Log Thread", false, false );
		}

		public override void Dispose() { }

		private void LogFunc() {
			if( !Directory.Exists( DirectoryPath ) )
				Directory.CreateDirectory( DirectoryPath );

			StringBuilder sb = new StringBuilder();
			string line;
			while( Mem.Threads.Running ) {
				sb.Clear();
				while( newLines.TryTake( out line, 100 ) ) {
					sb.Append( line );
				}

				bool attempting = true;
				while( attempting ) {
					try {
						File.AppendAllText( FilePath, sb.ToString() );
					} catch( Exception e ) {
						Mem.Logs.Error.WriteLine( e );
						if( !Directory.Exists( DirectoryPath ) )
							Directory.CreateDirectory( DirectoryPath );
					}
					attempting = false;
				}
			}

			bool rep = false;
			while( newLines.Count > 0 ) {
				sb.Clear();
				if( !rep )
					sb.Append( $"[{ DateTime.Now.ToString( "MM/dd/HH:mm:ss" ) }][{Thread.CurrentThread.Name}/{Thread.CurrentThread.CurrentCulture}]: PROGRAM EXIT!\n" );
				rep = true;
				while( newLines.TryTake( out line, 100 ) ) {
					sb.Append( line );
				}

				bool attempting = true;
				while( attempting ) {
					try {
						File.AppendAllText( FilePath, sb.ToString() );
					} catch( Exception e ) {
						MemLib.Mem.Logs.Error.WriteLine( e );
						if( !Directory.Exists( DirectoryPath ) )
							Directory.CreateDirectory( DirectoryPath );
					}
					attempting = false;
				}
			}

		}

		public void WriteLine( string text ) => WriteLine( text, DefaultFlags );

		public void WriteLine( string text, ConsoleColor color ) => WriteLine( text, color, DefaultFlags );

		public void WriteLine( string text, LogFlags flags ) => WriteLine( text, DefaultColor, flags );

		public void WriteLine( string text, ConsoleColor color, LogFlags flags ) {
			string s = $"[{ DateTime.Now.ToString( "MM/dd/HH:mm:ss" ) }][{Thread.CurrentThread.Name}/{Thread.CurrentThread.CurrentCulture}][{upperName}]: { text }";
			if( ( flags & LogFlags.STACK ) != 0 ) {
				StackTrace st = new StackTrace();
				s += "\r\n" + st.ToString();
			} else
				s += "\r\n";
			if( ( flags & LogFlags.QUIET ) == 0 && !Quiet ) {
				Console.ForegroundColor = color;
				Console.Write( s );
				Console.ForegroundColor = ConsoleColor.White;
			}
			newLines.Add( s );
		}

		public void Write( string text ) => Write( text, DefaultColor, DefaultFlags );

		public void Write( string text, ConsoleColor color ) => Write( text, color, DefaultFlags );

		public void Write( string text, LogFlags flags ) => Write( text, DefaultColor, flags );

		public void Write( string text, ConsoleColor color, LogFlags flags ) {
			string s = $"[{ DateTime.Now.ToString( "MM/dd/HH:mm:ss" ) }][{Thread.CurrentThread.Name}/{Thread.CurrentThread.CurrentCulture}][{upperName}]: { text }";
			if( ( flags & LogFlags.STACK ) != 0 ) {
				StackTrace st = new StackTrace();
				s += "\r\n" + st.ToString();
			}
			if( ( flags & LogFlags.QUIET ) == 0 && !Quiet ) {
				Console.ForegroundColor = color;
				Console.Write( s );
				Console.ForegroundColor = ConsoleColor.White;
			}
			newLines.Add( s );
		}
		public void WriteLine( object e ) => WriteLine( e.ToString(), DefaultFlags & ~LogFlags.STACK );

		public void WriteLine( object e, ConsoleColor color ) => WriteLine( e.ToString(), color, DefaultFlags & ~LogFlags.STACK );

		public void WriteLine( object e, LogFlags flags ) => WriteLine( e.ToString(), DefaultColor, flags );

		public void WriteLine( object e, ConsoleColor color, LogFlags flags ) => WriteLine( e.ToString(), color, flags );

		public void Write( object e ) => Write( e.ToString(), DefaultColor, DefaultFlags & ~LogFlags.STACK );

		public void Write( object e, ConsoleColor color ) => Write( e.ToString(), color, DefaultFlags & ~LogFlags.STACK );

		public void Write( object e, LogFlags flags ) => Write( e.ToString(), DefaultColor, flags );

		public void Write( object e, ConsoleColor color, LogFlags flags ) => Write( e.ToString(), color, flags );

	}

	public enum LogFlags {
		QUIET = 0x1,
		STACK = 0x2,
	}
}
