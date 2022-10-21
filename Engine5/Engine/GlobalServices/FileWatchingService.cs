using System.Collections.Concurrent;

namespace Engine.GlobalServices;

public sealed class FileWatchingService : Identifiable, IDisposable, IGlobalService {

	private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers;
	private readonly ConcurrentDictionary<string, Action> _trackedFilepaths;


	public FileWatchingService() {
		_watchers = new();
		_trackedFilepaths = new();
	}

	public void Track( string filePath, Action callback ) {
		string directoryPath = Path.GetDirectoryName( filePath ) ?? throw new NullReferenceException();
		if ( !_watchers.TryGetValue( directoryPath, out FileSystemWatcher? watcher ) ) {
			_watchers.TryAdd( directoryPath, watcher = new FileSystemWatcher( filePath ) );
			watcher.Changed += FileChanged;
		}
		_trackedFilepaths.TryAdd( filePath, callback );
	}


	public void Untrack( string filePath ) => _trackedFilepaths.TryRemove( filePath, out _ );

	private void FileChanged( object sender, FileSystemEventArgs e ) {
		if ( _trackedFilepaths.TryGetValue( Path.GetRelativePath( Environment.CurrentDirectory, e.FullPath ), out var callback ) )
			callback?.Invoke();
	}

	public void Dispose() {



	}
}