namespace Engine.Structures;

public sealed partial class TypeDigraph {
	public static partial class Sort {
		/// <summary>
		/// Does not allocate any memory, but get slower when the number of types is large.
		/// </summary>
		public static class NoAlloc {
			private readonly struct NodeReference( int nodeA, int nodeB, bool nodeAIsParent ) {
				public readonly int ParentIndex = nodeAIsParent ? nodeA : nodeB;
				public readonly int ChildIndex = nodeAIsParent ? nodeB : nodeA;

				public bool IsEqual( int parent, int child ) => this.ParentIndex == parent && this.ChildIndex == child;
				public bool IsEqual( int nodeA, int nodeB, bool nodeAIsParent ) => nodeAIsParent ? IsEqual( nodeA, nodeB ) : IsEqual( nodeB, nodeA );

				public override string ToString() => $"{this.ParentIndex} -> {this.ChildIndex}";
			}

			public static void Sort( Type processType, HashSet<Type> types, List<ResolvedType> unorderedTypes, List<Type> orderedTypes ) {
				int numActiveReferences = 0;
				Span<NodeReference> references = stackalloc NodeReference[ unorderedTypes.Count * (unorderedTypes.Count - 1) ];

				for (int node = 0; node < unorderedTypes.Count; node++) {
					int numReferences = 0;
					IReadOnlyList<Process.IProcessDirection> attributes = unorderedTypes[ node ].GetAttributes<Process.IProcessDirection>();
					for (int i = 0; i < attributes.Count; i++) {
						Process.IProcessDirection attribute = attributes[ i ];
						if (!(processType == attribute.ProcessType))
							continue;
						Type? expectedType = null;
						bool nodeIsParent = false;
						if (attribute is Process.IProcessBefore beforeAttribute) {
							expectedType = beforeAttribute.BeforeType;
							nodeIsParent = true;
						} else if (attribute is Process.IProcessAfter afterAttribute)
							expectedType = afterAttribute.AfterType;
						if (expectedType is null)
							continue;
						if (!types.Contains( expectedType ))
							continue;

						int? referencedNodeIndex = null;
						for (int j = 0; j < unorderedTypes.Count; j++) {
							if (node != j && unorderedTypes[ j ].Type == expectedType) {
								referencedNodeIndex = j;
								break;
							}
						}

						if (!referencedNodeIndex.HasValue)
							continue;

						bool foundReference = false;
						for (int j = 0; j < numActiveReferences; j++) {
							if (references[ j ].IsEqual( node, referencedNodeIndex.Value, nodeIsParent )) {
								foundReference = true;
								break;
							}
						}

						if (foundReference)
							continue;

						references[ numActiveReferences++ ] = new( node, referencedNodeIndex.Value, nodeIsParent );
						numReferences++;
					}
				}

				Span<(int start, int length)> parentReferenceRanges = stackalloc (int start, int length)[ unorderedTypes.Count ];
				Span<(int start, int length)> childReferenceRanges = stackalloc (int start, int length)[ unorderedTypes.Count ];
				Span<NodeReference> referencesOrderedByParentIndex = stackalloc NodeReference[ numActiveReferences ];
				Span<NodeReference> referencesOrderedByChildIndex = stackalloc NodeReference[ numActiveReferences ];

				for (int i = 0; i < numActiveReferences; i++) {
					referencesOrderedByParentIndex[ i ] = references[ i ];
					referencesOrderedByChildIndex[ i ] = references[ i ];
				}

				referencesOrderedByParentIndex.Sort( ( a, b ) => a.ParentIndex - b.ParentIndex );
				referencesOrderedByChildIndex.Sort( ( a, b ) => a.ChildIndex - b.ChildIndex );

				int currentNodeIndex = 0;
				int start = 0;
				for (int i = 0; i < numActiveReferences; i++) {
					if (referencesOrderedByParentIndex[ i ].ParentIndex != currentNodeIndex) {
						parentReferenceRanges[ currentNodeIndex ] = (start, i - start);
						start = i;
						currentNodeIndex = referencesOrderedByParentIndex[ i ].ParentIndex;
					}
				}
				parentReferenceRanges[ currentNodeIndex ] = (start, numActiveReferences - start);
				currentNodeIndex = 0;
				start = 0;
				for (int i = 0; i < numActiveReferences; i++) {
					if (referencesOrderedByChildIndex[ i ].ChildIndex != currentNodeIndex) {
						childReferenceRanges[ currentNodeIndex ] = (start, i - start);
						start = i;
						currentNodeIndex = referencesOrderedByChildIndex[ i ].ChildIndex;
					}
				}
				childReferenceRanges[ currentNodeIndex ] = (start, numActiveReferences - start);

				Span<int> nodeStates = stackalloc int[ unorderedTypes.Count ];
				for (int i = 0; i < unorderedTypes.Count; i++) {
					if (nodeStates[ i ] == 2)
						continue;
					if (HasCycle( nodeStates, referencesOrderedByParentIndex, parentReferenceRanges, i ))
						throw new InvalidOperationException( $"Circular reference detected. Hint: {unorderedTypes[ i ].Type.Name}." );
				}

				orderedTypes.Clear();
				Span<int> placedIndices = stackalloc int[ unorderedTypes.Count ];
				placedIndices.Fill( -1 );
				int numPlacedIndices = 0;
				SpanQueue<int> indicesToPlace = new( stackalloc int[ unorderedTypes.Count ] );
				for (int i = 0; i < unorderedTypes.Count; i++) {
					bool hasParent = false;
					for (int j = 0; j < childReferenceRanges[ i ].length; j++) {
						int referenceIndex = childReferenceRanges[ i ].start + j;
						if (referencesOrderedByChildIndex[ referenceIndex ].ChildIndex == i) {
							hasParent = true;
							break;
						}
					}
					if (hasParent)
						continue;
					indicesToPlace.Enqueue( i );
				}

				while (indicesToPlace.TryDequeue( out int index )) {
					if (placedIndices.Contains( index ))
						continue;
					//Check if all parents are placed
					bool allParentsPlaced = true;
					for (int j = 0; j < childReferenceRanges[ index ].length; j++) {
						int referenceIndex = childReferenceRanges[ index ].start + j;
						NodeReference reference = referencesOrderedByChildIndex[ referenceIndex ];
						if (reference.ChildIndex == index) {
							if (!placedIndices.Contains( reference.ParentIndex )) {
								allParentsPlaced = false;
								break;
							}
						}
					}
					if (!allParentsPlaced) {
						indicesToPlace.Enqueue( index );
						continue;
					}

					orderedTypes.Add( unorderedTypes[ index ].Type );
					placedIndices[ numPlacedIndices++ ] = index;
					for (int j = 0; j < parentReferenceRanges[ index ].length; j++) {
						int referenceIndex = parentReferenceRanges[ index ].start + j;
						NodeReference reference = referencesOrderedByParentIndex[ referenceIndex ];
						if (reference.ParentIndex == index) {
							if (placedIndices.Contains( reference.ChildIndex ))
								continue;
							indicesToPlace.Enqueue( reference.ChildIndex );
						}
					}
				}
			}

			private static bool HasCycle( Span<int> nodeStates, Span<NodeReference> references, Span<(int start, int length)> parentReferenceRanges, int nodeIndex ) {
				if (nodeStates[ nodeIndex ] == 1)
					return true;
				if (nodeStates[ nodeIndex ] == 2)
					return false;
				nodeStates[ nodeIndex ] = 1;
				for (int j = 0; j < parentReferenceRanges[ nodeIndex ].length; j++) {
					int referenceIndex = parentReferenceRanges[ nodeIndex ].start + j;
					if (references[ referenceIndex ].ParentIndex != nodeIndex)
						continue;
					if (HasCycle( nodeStates, references, parentReferenceRanges, references[ referenceIndex ].ChildIndex ))
						return true;
				}
				nodeStates[ nodeIndex ] = 2;
				return false;
			}
		}
	}
}