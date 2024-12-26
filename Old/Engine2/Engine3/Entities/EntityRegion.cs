using Engine.Utilities.Data.Boxing;
using System.Collections.Generic;

namespace Engine.Entities {
	public class EntityRegion {

		#region ID
		private static uint GetNextID() {
			lock( openID )
				return openID.Value++;
		}
		private static readonly MutableSinglet<uint> openID = new MutableSinglet<uint>( 0 );
		#endregion

		public readonly uint RegionId;

		private HashSet<Entity> entities;
		public IReadOnlyCollection<Entity> Entities { get => entities; }

		public EntityRegion() {
			RegionId = GetNextID();
			entities = new HashSet<Entity>();
		}

		internal bool Add( Entity e ) {
			return entities.Add( e );
		}

		internal bool Remove( Entity e ) {
			return entities.Remove( e );
		}
	}
}