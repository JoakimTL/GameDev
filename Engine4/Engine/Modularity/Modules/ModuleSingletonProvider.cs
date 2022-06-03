using System.Reflection;
using Engine.Structure;

namespace Engine.Modularity.Modules;
public class ModuleSingletonProvider : UpdateableSingletonProvider<ModuleSingletonBase> {

	private readonly FieldInfo _singletonOwnerField;
	private readonly Module _parent;

	internal ModuleSingletonProvider( Module parent ) : base() {
		this._parent = parent;
		this._singletonOwnerField = typeof( ModuleSingletonBase ).GetField( "_owner", BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new NullReferenceException( "Unable to find singleton owner field." );
		SingletonAdded += CheckModuleType;
	}

	private void CheckModuleType( object obj ) {
		if ( obj is ModuleSingletonBase singleton )
			this._singletonOwnerField.SetValue( singleton, this._parent );
	}
}

