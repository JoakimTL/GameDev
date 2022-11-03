using Engine.Structure.Interfaces;
using Engine.Time;
using System.Collections.Concurrent;

namespace Engine.GlobalServices;

public sealed class ReferenceCountingService : Identifiable, IGlobalService {

	private readonly ConcurrentDictionary<object, List<WeakReference>> _references;
	private readonly TickingTimer _trimTimer;

	public event Action<object>? Unreferenced;

	public ReferenceCountingService() {
		_references = new();
		_trimTimer = new( "Reference Trimming", 10000 );
		_trimTimer.Elapsed += Trim;
		_trimTimer.Start();
	}

	public void Reference( object referee, object referenced ) {
		if ( !_references.TryGetValue( referenced, out var list ) )
			_references.TryAdd( referenced, list = new() );
		lock ( list )
			list.Add( new WeakReference( referee ) );
	}

	public void Dereference( object referee, object referenced ) {
		if ( _references.TryGetValue( referenced, out var list ) )
			lock ( list ) {
				int removed = list.RemoveAll( p => p.Target is null || p.Target == referee );
				if ( removed > 0 )
					this.LogLine( $"Removed {removed} references to {referenced}. Triggered by {referee}.", Log.Level.NORMAL );
			}
		CheckUnreferenced( referenced );
	}

	/// <summary>
	/// Not a precise count, as dereferenced objects might still linger as weak references untill trimmed.
	/// </summary>
	/// <param name="referenced"></param>
	/// <returns></returns>
	public int GetReferenceCount( object referenced ) {
		if ( _references.TryGetValue( referenced, out var list ) )
			lock ( list )
				return list.Count;
		return 0;
	}

	private void Trim( double time, double deltaTime ) {
		foreach ( var kvp in _references ) {
			var list = kvp.Value;
			lock ( list ) {
				int removed = list.RemoveAll( p => p.Target is null );
				if ( removed > 0 )
					this.LogLine( $"Trimmed {removed} references to {kvp.Key}.", Log.Level.NORMAL );
			}
			CheckUnreferenced( kvp.Key );
		}
	}

	private void CheckUnreferenced( object referenced ) {
		if ( _references.TryGetValue( referenced, out var list ) )
			lock ( list )
				if ( list.Count == 0 ) {
					_references.Remove( referenced, out _ );
					Unreferenced?.Invoke( referenced );
				}
	}
}
