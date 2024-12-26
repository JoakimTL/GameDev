using Engine.Utilities.Data.Boxing;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Engine.Entities {
	public class Entity {

		#region ID
		private static uint GetNextID() {
			lock( openID )
				return openID.Value++;
		}
		private static readonly MutableSinglet<uint> openID = new MutableSinglet<uint>( 0 );
		#endregion

		public readonly uint UID;
		public string Name { get; private set; }

		private Dictionary<Type, Module> modules;

		public delegate void EntityModuleChangeHandler( Entity e, Module m );
		public event EntityModuleChangeHandler ModuleRemoved;
		public event EntityModuleChangeHandler ModuleAdded;

		public Entity( string name ) {
			UID = GetNextID();
			Name = name;
			modules = new Dictionary<Type, Module>();
		}

		public bool Get<T>( out T module ) where T : Module {
			module = default;
			if( modules.TryGetValue( typeof( T ), out Module m ) ) {
				module = m as T;
				return true;
			}
			return false;
		}

		public void Update( float time, float deltaTime ) {
			foreach( Module module in modules.Values ) {
				module.Update( time, deltaTime );
			}
		}

		//Create a dynamic update system?
		//Allowing for multiple update steps, and ensuring updates after certain modules if they exist

		public bool Add( Module m ) {
			if( m is null ) {
				Logging.Warning( $"Module is null!" );
				return false;
			}
			Type mType = m.GetType();
			if( !modules.ContainsKey( mType ) && m.SetOwner( this ) ) {
				modules.Add( mType, m );
				ModuleAdded?.Invoke(this,  m );
				return true;
			}
			Logging.Warning( $"Module [{m}] rejected!" );
			return false;
		}

		public bool Remove( Module m ) {
			return Remove( m.GetType() );
		}

		public bool Remove( Type t ) {
			if( modules.TryGetValue( t, out Module mr ) ) {
				modules.Remove( t );
				ModuleRemoved?.Invoke( this, mr );
				return true;
			}
			return false;
		}

		public override bool Equals( object obj ) {
			if( obj is Entity e )
				return e.UID == UID;
			return false;
		}

		public override int GetHashCode() {
			return (int) UID;
		}

		public override string ToString() {
			string list = "";
			foreach( KeyValuePair<Type, Module> kvp in modules ) {
				list += $"[{kvp.Key.Name},{kvp.Value}]";
			}
			return $"Entity[{UID},{Name}][{list}]";
		}

	}
}
