using System.Collections.Concurrent;
using Engine.Structure.Interfaces;

namespace Engine.GlobalServices;

public sealed class FileWatchingService : Identifiable, IDisposable, IGlobalService {

	private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers;
	private readonly ConcurrentDictionary<string, HashSet<Action<string>>> _trackedFilepaths;


	public FileWatchingService() {
		_watchers = new();
		_trackedFilepaths = new();
	}

	public void Track( string filePath, Action<string> callback ) {
		string directoryPath = Path.GetDirectoryName( filePath ) ?? throw new NullReferenceException();
		if ( !_watchers.TryGetValue( directoryPath, out FileSystemWatcher? watcher ) ) {
			_watchers.TryAdd( directoryPath, watcher = new FileSystemWatcher( filePath ) );
			watcher.Changed += FileChanged;
		}
		if ( !_trackedFilepaths.TryGetValue( filePath, out var trackers ) )
			_trackedFilepaths.TryAdd( filePath, trackers = new() );
		trackers.Add( callback );
	}


	public void Untrack( string filePath, Action<string> callback ) {
		if ( _trackedFilepaths.TryGetValue( filePath, out var trackers ) )
			trackers.Remove( callback );
	}

	private void FileChanged( object sender, FileSystemEventArgs e ) {
		string relativePath = Path.GetRelativePath( Environment.CurrentDirectory, e.FullPath );
		if ( _trackedFilepaths.TryGetValue( relativePath, out var callbacks ) )
			foreach ( var tracker in callbacks )
				tracker?.Invoke( relativePath );
	}

	public void Dispose() {
		foreach ( var watcher in _watchers.Values )
			watcher.Dispose();
	}
}

//TODO FileReadingService
//To read packed data or native file directories
//public sealed class FileReadingService 