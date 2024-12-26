using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public class UIEvent {
		private readonly HashSet<uint> elementSet;
		private readonly Dictionary<uint, HashSet<uint>> activatedElements;

		//Possible future?
		//private Dictionary<int, HashSet<uint>> elementsAboveLayer;
		//private List<int> activeLayers;

		public uint DeepestLayer {
			get; private set;
		}
		private bool active;

		public UIEvent() {
			elementSet = new HashSet<uint>();
			activatedElements = new Dictionary<uint, HashSet<uint>>();
			DeepestLayer = uint.MinValue;
			active = false;
		}

		public void Add( UIElement e ) {
			if( !activatedElements.TryGetValue( e.Layer, out HashSet<uint> elements ) )
				activatedElements.Add( e.Layer, elements = new HashSet<uint>() );
			elements.Add( e.ID );
			elementSet.Add( e.ID );

			active = true;
			if( e.Layer > DeepestLayer )
				DeepestLayer = e.Layer;
		}

		public void Reset() {
			foreach( HashSet<uint> ele in activatedElements.Values )
				ele.Clear();
			DeepestLayer = uint.MinValue;
			active = false;
		}

		public bool HasElement() {
			return elementSet.Count > 0;
		}

		public HashSet<uint> GetElements( uint layer ) {
			if( activatedElements.TryGetValue( layer, out HashSet<uint> ele ) )
				return ele;
			return new HashSet<uint>();
		}

		public bool ElementExistsInLayer( uint layer, HashSet<UIElement> set, HashSet<uint> whitelist ) {
			if( !active )
				return false;

			uint curLayer = layer;
			HashSet<uint> activatedLayer = null;
			foreach( UIElement e in set ) {
				if( e.Layer > layer ) {
					if( whitelist.Contains( e.ID ) )
						continue;
					if( curLayer != e.Layer || activatedLayer == null ) {
						if( activatedElements.TryGetValue( e.Layer, out activatedLayer ) )
							if( activatedLayer.Contains( e.ID ) )
								return true;
					} else {
						if( activatedLayer.Contains( e.ID ) )
							return true;
					}
				}
			}
			return false;
		}

		public bool ElementExists( HashSet<UIElement> set, HashSet<uint> whitelist ) {
			if( !active )
				return false;

			foreach( UIElement e in set ) {
				if( whitelist.Contains( e.ID ) )
					continue;
				if( elementSet.Contains( e.ID ) )
					return true;
			}
			return false;
		}
	}
}
