namespace Engine.Modularity.Modules.Singletons;

public class Userdata : ModuleSingletonBase {

	public bool UsernameSet { get; private set; }
	public string Username { get; private set; }

	public Userdata() {
		this.UsernameSet = false;
		this.Username = "_Unknown_";
	}

	public void SetUsername( string newUsername ) {
		if ( string.IsNullOrEmpty( newUsername ) )
			return;
		if ( newUsername == "_Server_" || newUsername == "_Unknown_" ) {
			this.LogWarning( "Username not available. Choose another." );
			return;
		}
		if ( newUsername == this.Username )
			return;
		this.UsernameSet = true;
		this.Username = newUsername;
	}

	protected override bool OnDispose() => true;
}
