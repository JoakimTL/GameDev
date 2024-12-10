using Engine.Processing;

namespace Engine.Structures;

public sealed partial class TypeDigraph {
	public static partial class Sort {
		/// <summary>
		/// Allocates memory, but is faster when the number of types is large.
		/// </summary>
		public static class Alloc {
			private sealed class Node( ResolvedType resolvedType ) {
				public readonly ResolvedType ResolvedType = resolvedType;
				public readonly HashSet<Type> Children = [];
				public readonly HashSet<Type> Parents = [];
				public bool PlaceLast;

				public NodeState State;
				public enum NodeState { Unvisited, Visiting, Processed }

				public override string ToString() => $"{this.ResolvedType.Type.Name} ({string.Join( ", ", this.Parents.Select( p => p.Name ) )}) -> ({string.Join( ", ", this.Children.Select( p => p.Name ) )})";
			}

			public static void Sort( Type processType, List<ResolvedType> unorderedTypes, List<Type> orderedTypes ) {
				Dictionary<Type, Node> nodes = unorderedTypes.Select( p => new Node( p ) ).ToDictionary( p => p.ResolvedType.Type );

				foreach (Node? node in nodes.Values) {
					List<IProcessDirection> relevantAttributes = node.ResolvedType.GetAttributes<IProcessDirection>().Where( p => p.ProcessType == processType ).ToList();
					foreach (IProcessBefore beforeAttribute in relevantAttributes.OfType<IProcessBefore>()) {
						if (!nodes.TryGetValue( beforeAttribute.BeforeType, out Node? beforeNode ))
							continue;
						beforeNode.Parents.Add( node.ResolvedType.Type );
						node.Children.Add( beforeNode.ResolvedType.Type );
					}
					foreach (IProcessAfter afterAttribute in relevantAttributes.OfType<IProcessAfter>()) {
						if (!nodes.TryGetValue( afterAttribute.AfterType, out Node? afterNode ))
							continue;
						node.Parents.Add( afterNode.ResolvedType.Type );
						afterNode.Children.Add( node.ResolvedType.Type );
					}
					if (relevantAttributes.OfType<IProcessLast>().Any())
						node.PlaceLast = true;
				}

				foreach (Node? node in nodes.Values) {
					if (node.State == Node.NodeState.Processed)
						continue;
					if (HasCycle( nodes, node ))
						throw new InvalidOperationException( $"Circular reference detected at {node.ResolvedType.Type.Name}." );
				}

				orderedTypes.Clear();
				HashSet<Type> placedTypes = [];
				Queue<Node> nodesToBePlaced = new( nodes.Values.Where( p => p.Parents.Count == 0 && !p.PlaceLast ) );

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

				foreach (Node? node in nodes.Values.Where( p => p.PlaceLast ))
					nodesToBePlaced.Enqueue( node );

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