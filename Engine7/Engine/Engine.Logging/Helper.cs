using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Engine.Logging.Log;

namespace Engine.Logging;

internal static class Helper {
	public static string GenerateStackTrace( int level ) {
		StackTrace stack = new( true );
		int startLevel = level + 1;
		int maxPaddingLength = 0;
		for (int i = startLevel; i < stack.FrameCount; i++) {
			StackFrame? sf = stack.GetFrame( i );
			if (sf is null)
				continue;
			MethodBase? mth = sf.GetMethod();
			if (mth is null)
				continue;
			string fullName = mth.DeclaringType?.ReflectedType?.FullName ?? "";
			if (fullName.Length > maxPaddingLength)
				maxPaddingLength = fullName.Length;
		}

		string output = "";
		for (int i = startLevel; i < stack.FrameCount; i++) {
			StackFrame? sf = stack.GetFrame( i );
			MethodBase? mth = sf?.GetMethod();
			string fullName = mth?.DeclaringType?.ReflectedType?.FullName ?? "";
			string methodName = mth?.Name ?? "External code...";
			string lineInformation = sf is not null ? $"[{sf.GetFileLineNumber()}:{sf.GetFileColumnNumber()}]" : "";
			string spacing = new( ' ', maxPaddingLength - fullName.Length );
			output += $" @ {fullName}{spacing}.{methodName}{lineInformation}";
			if (i < stack.FrameCount - 1)
				output += Environment.NewLine;
		}

		return output;
	}

	public static unsafe void SendMessageOverPipe( NamedPipeServerStream pipeServer, string message ) {
		if (!pipeServer.IsConnected)
			return;

		byte* dstPtr = stackalloc byte[ ushort.MaxValue + 1 ];
		uint writtenBytes = 0;
		uint attempts = 0;
		while (writtenBytes < message.Length && attempts < 10) {
			uint bytesToBeSent = (uint) Math.Min( message.Length * sizeof( char ) - writtenBytes, ushort.MaxValue );
			fixed (char* srcPtr = message)
				Unsafe.CopyBlock( dstPtr, srcPtr + writtenBytes, bytesToBeSent );

			ReadOnlySpan<byte> buffer = new( dstPtr, (int) bytesToBeSent );
			try {
				pipeServer.Write( buffer );
				writtenBytes += bytesToBeSent;
				attempts = 0;
			} catch (Exception e) {
				Critical( e );
				attempts++;
			}
		}
		if (attempts >= 10)
			Warning( "Stopped sending log data to pipe." );
	}

	public static string GetLogPrefix() => $"{Thread.CurrentThread.Name}:{Environment.CurrentManagedThreadId}/{Thread.CurrentThread.CurrentCulture.Name}|{DateTime.Now:HHmm:ss.f}";
}


