namespace Engine;

public partial class TypeDigraph {
	public static partial class Sort {
		/// <summary>
		/// Allocates memory, but is faster when the number of types is large.
		/// </summary>
		public static class Alloc {
			private class Node( ResolvedType resolvedType ) {
				public ResolvedType ResolvedType = resolvedType;
				public HashSet<Type> Children = [];
				public HashSet<Type> Parents = [];

				public NodeState State;
				public enum NodeState { Unvisited, Visiting, Processed }

				public override string ToString() => $"{this.ResolvedType.Type.Name} ({string.Join( ", ", this.Parents.Select( p => p.Name ) )}) -> ({string.Join( ", ", this.Children.Select( p => p.Name ) )})";
			}

			public static void Sort( Type processType, List<ResolvedType> unorderedTypes, List<Type> orderedTypes ) {
				var nodes = unorderedTypes.Select( p => new Node( p ) ).ToDictionary( p => p.ResolvedType.Type );

				foreach (var node in nodes.Values) {
					var relevantAttributes = node.ResolvedType.GetAttributes<Process.IProcessDirection>().Where( p => p.ProcessType == processType );
					foreach (var beforeAttribute in relevantAttributes.OfType<Process.IProcessBefore>()) {
						if (!nodes.TryGetValue( beforeAttribute.BeforeType, out Node? beforeNode ))
							continue;
						beforeNode.Parents.Add( node.ResolvedType.Type );
						node.Children.Add( beforeNode.ResolvedType.Type );
					}
					foreach (var afterAttribute in relevantAttributes.OfType<Process.IProcessAfter>()) {
						if (!nodes.TryGetValue( afterAttribute.AfterType, out Node? afterNode ))
							continue;
						node.Parents.Add( afterNode.ResolvedType.Type );
						afterNode.Children.Add( node.ResolvedType.Type );
					}
				}

				foreach (var node in nodes.Values) {
					if (node.State == Node.NodeState.Processed)
						continue;
					if (HasCycle( nodes, node ))
						throw new InvalidOperationException( $"Circular reference detected at {node.ResolvedType.Type.Name}." );
				}

				orderedTypes.Clear();
				HashSet<Type> placedTypes = [];
				Queue<Node> nodesToBePlaced = new( nodes.Values.Where( p => p.Parents.Count == 0 ) );

				while (nodesToBePlaced.TryDequeue( out Node? node )) {
					if (placedTypes.Contains( node.ResolvedType.Type ))
						continue;

					if (!node.Parents.All( placedTypes.Contains )) {
						nodesToBePlaced.Enqueue( node );
						if (nodesToBePlaced.Count == 1)
							return;
						continue;
					}

					orderedTypes.Add( node.ResolvedType.Type );
					placedTypes.Add( node.ResolvedType.Type );
					foreach (Type child in node.Children)
						nodesToBePlaced.Enqueue( nodes[ child ] );
				}
			}

			private static bool HasCycle( Dictionary<Type, Node> nodes, Node node ) {
				if (node.State == Node.NodeState.Visiting)
					return true;
				if (node.State == Node.NodeState.Processed)
					return false;
				node.State = Node.NodeState.Visiting;
				foreach (Type child in node.Children) {
					if (!nodes.TryGetValue( child, out Node? childNode ))
						continue;
					if (HasCycle( nodes, childNode ))
						return true;
				}
				node.State = Node.NodeState.Processed;
				return false;
			}
		}
	}
}