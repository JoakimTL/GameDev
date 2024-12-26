using Engine.Entities.D3;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LinearAlgebra;
using Engine.Utilities.Data;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Entities {

	public class Entity3Manager : EntityManager {

		private int regionSize;
		private float regionUpdateInterval;

		private Scene<SceneObjectData3> scene;

		private Dictionary3Layered<int, Entity3Region> regionByPosition;
		private Dictionary<uint, Entity3Region> regionByEntityId;
		private Dictionary<uint, Entity> entitiesById;
		private HashSet<Entity> entities;

		public IReadOnlyCollection<Entity> Entities { get => entities; }
		private float lastRegionUpdate;

		/// <summary>
		/// Contains and manages entities. Think of the entitymanager as a worldspace
		/// </summary>
		/// <param name="scene">The scene all the entity render modules will be placed in.</param>
		/// <param name="regionUpdateInterval">The interval at which the entity region is corrected at.</param>
		/// <param name="regionSize">The size of the region. A lower amount will increase memory usage, but decrease computation time.</param>
		public Entity3Manager( Scene<SceneObjectData3> scene, float regionUpdateInterval = 0.1f, int regionSize = 32 ) {
			this.regionSize = regionSize;
			this.regionUpdateInterval = regionUpdateInterval;
			this.scene = scene;
			regionByPosition = new Dictionary3Layered<int, Entity3Region>();
			regionByEntityId = new Dictionary<uint, Entity3Region>();
			entitiesById = new Dictionary<uint, Entity>();
			entities = new HashSet<Entity>();
			lastRegionUpdate = -1;
		}

		public override void Add( Entity e ) {
			entities.Add( e );
			entitiesById.Add( e.UID, e );
			ScanModules( e );
			e.ModuleRemoved += EntityModuleRemoved;
			e.ModuleAdded += EntityModuleAdded;
		}

		public override void Remove( Entity e ) {
			entities.Remove( e );
			entitiesById.Remove( e.UID );
			regionByEntityId.Remove( e.UID );
			e.ModuleAdded -= EntityModuleAdded;
			e.ModuleRemoved -= EntityModuleRemoved;
		}

		private void ScanModules(Entity e) {
			if( e.Get( out Render3Module rm ) )
				scene.Add( rm.RenderObject );
		}

		private void EntityModuleAdded( Entity e, Module m ) {
			if (m is Render3Module rm )
				scene.Add( rm.RenderObject );
			}

		private void EntityModuleRemoved( Entity e, Module m ) {
			if( m is Render3Module rm )
				scene.Remove( rm.RenderObject );
		}

		public override void Update( float time, float deltaTime ) {
			if( time - lastRegionUpdate > regionUpdateInterval ) {
				foreach( Entity e in entities )
					UpdateRegion( e );
				lastRegionUpdate = time;
			}
			foreach( Entity e in entities ) {
				e.Update( time, deltaTime );
			}
		}

		private void UpdateRegion( Entity e ) {
			if( e.Get( out Transform3Module tm ) ) {
				if( regionByEntityId.TryGetValue( e.UID, out Entity3Region currentRegion ) ) {
					Vector3i regionPosition = ( tm.Transform.GlobalTranslation / regionSize ).IntFloored;
					if( currentRegion.Position != regionPosition ) {
						currentRegion.Remove( e );
						Entity3Region region = GetRegion( regionPosition );
						region.Add( e );
						regionByEntityId[ e.UID ] = region;
					}
				} else {
					Entity3Region region = GetRegion( tm.Transform.GlobalTranslation );
					region.Add( e );
					regionByEntityId[ e.UID ] = region;
				}
			}
		}

		public Entity3Region GetRegion( Entity e ) {
			if( regionByEntityId.TryGetValue( e.UID, out Entity3Region region ) )
				return region;
			return null;
		}

		/// <summary>
		/// Returns the entity region corresponding to the input region coordinate.
		/// </summary>
		/// <param name="regionPosition">The region to fetch from.</param>
		/// <returns>The region containing entities</returns>
		public Entity3Region GetRegion( Vector3i regionPosition ) {
			if( !regionByPosition.TryGet( regionPosition.X, regionPosition.Y, regionPosition.Z, out Entity3Region region ) )
				regionByPosition.Add( regionPosition.X, regionPosition.Y, regionPosition.Z, region = new Entity3Region( regionPosition ) );
			return region;
		}

		/// <summary>
		/// Returns the entity region corresponding to the input world coordinate.
		/// </summary>
		/// <param name="translation">The world coordinate.</param>
		/// <returns>The region containing entities</returns>
		public Entity3Region GetRegion( Vector3 translation ) {
			Vector3i regionPosition = ( translation / regionSize ).IntFloored;
			return GetRegion( regionPosition );
		}

		public bool GetFromId( uint id, out Entity e ) {
			if( entitiesById.TryGetValue( id, out e ) )
				return true;
			return false;
		}

		/// <summary>
		/// Inserts entities into the provided hashset. Entities already in the hashset is ignored. Returns the amount of inserted entities.<br></br>
		/// <b>Does NOT remove existing elements.</b>
		/// </summary>
		/// <param name="center">Center of the sphere of range</param>
		/// <param name="radius">Radius in world units</param>
		/// <param name="entities">The collection the entities should be put into</param>
		/// <returns>The number of new entities added to the list.</returns>
		public HashSet<Entity> GetEntitiesInRange( Vector3 center, float radius ) {
			HashSet<Entity> entities = new HashSet<Entity>();
			for( float x = center.X - radius; x < center.X + radius; x += radius ) {
				for( float y = center.Y - radius; y < center.Y + radius; y += radius ) {
					for( float z = center.Z - radius; z < center.Z + radius; z += radius ) {
						Entity3Region r = GetRegion( new Vector3( x, y, z ) );
						if( r is null )
							continue;
						foreach( Entity e in r.Entities ) {
							if( e.Get( out Transform3Module tm ) )
								if( ( tm.Transform.GlobalTranslation - center ).LengthSquared < radius * radius ) {
									entities.Add( e );
								}
						}
					}
				}
			}
			return entities;
		}

		/// <summary>
		/// Inserts entities into the provided hashset. Entities already in the hashset is ignored. Returns the amount of inserted entities.<br></br>
		/// <b>Does NOT remove existing elements.</b>
		/// </summary>
		/// <param name="center">Center of the sphere of range</param>
		/// <param name="radius">Radius in world units</param>
		/// <param name="entities">The collection the entities should be put into</param>
		/// <returns>The number of new entities added to the list.</returns>
		public int GetEntitiesInRange( Vector3 center, float radius, HashSet<Entity> entities ) {
			int newEntries = 0;
			for( float x = center.X - radius; x < center.X + radius; x += radius ) {
				for( float y = center.Y - radius; y < center.Y + radius; y += radius ) {
					for( float z = center.Z - radius; z < center.Z + radius; z += radius ) {
						Entity3Region r = GetRegion( new Vector3( x, y, z ) );
						if( r is null )
							continue;
						foreach( Entity e in r.Entities ) {
							if( e.Get( out Transform3Module tm ) )
								if( ( tm.Transform.GlobalTranslation - center ).LengthSquared < radius * radius ) {
									if( entities.Add( e ) )
										newEntries++;
								}
						}
					}
				}
			}
			return newEntries;
		}

	}
}
