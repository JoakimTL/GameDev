using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Entities {
	public abstract class EntityManager {

		//Entity Manager should be a sealed class.
		//Additional classes to manage 3d and 2d entities, from the same manager even.
		/*
		 * Entities should have the following identifiers:
		 *	- Tags
		 *	- Names
		 *	- UIDs
		 *	- Components
		 *	
		 *	The manager should be able to fetch entities from all those identifiers.
		 *	
		 *	The spacial managers are essentially quad- and oct-trees, sorting entities into subsections to make access to surrounding entities faster.
		 */

		public abstract void Add( Entity e );
		public abstract void Remove( Entity e );
		public abstract void Update( float time, float deltaTime );
	}
}
