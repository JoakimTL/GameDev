using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Tests;

[TestFixture]
public sealed class BidirectionTypeTreeTests {

	private class ProcessType;

	[Direction.Before<TypeSeq2_1, ProcessType>]
	private class TypeSeq1_1;
	[Direction.Before<TypeSeq2_2, ProcessType>]
	private class TypeSeq1_2;

	private class TypeSeq2_1;
	private class TypeSeq2_2;

	[Direction.After<TypeSeq2_1, ProcessType>]
	private class TypeSeq3_1;
	[Direction.After<TypeSeq2_2, ProcessType>]
	private class TypeSeq3_2;

	[Test]
	public void StandardTreeTest() {
		BidirectionalTypeTree<ProcessType> tree = new();

		tree.Add( typeof( TypeSeq3_1 ) );
		tree.Add( typeof( TypeSeq2_1 ) );
		tree.Add( typeof( TypeSeq1_1 ) );

		List<Type> nodes = tree.UpdateAndGetNodesSorted().ToList();

		Assert.That( nodes.Count, Is.EqualTo( 3 ) );
		Assert.That( nodes[ 0 ], Is.EqualTo( typeof( TypeSeq1_1 ) ) );
		Assert.That( nodes[ 1 ], Is.EqualTo( typeof( TypeSeq2_1 ) ) );
		Assert.That( nodes[ 2 ], Is.EqualTo( typeof( TypeSeq3_1 ) ) );
	}

	[Test]
	public void StandardTreeWithOrphanTest() {
		BidirectionalTypeTree<ProcessType> tree = new();

		tree.Add( typeof( TypeSeq3_1 ) );
		tree.Add( typeof( TypeSeq2_1 ) );
		tree.Add( typeof( TypeSeq2_2 ) );
		tree.Add( typeof( TypeSeq1_1 ) );

		List<Type> nodes = tree.UpdateAndGetNodesSorted().ToList();

		Assert.That( nodes.Count, Is.EqualTo( 4 ) );
		Assert.That( nodes[ 0 ], Is.EqualTo( typeof( TypeSeq2_2 ) ) );
		Assert.That( nodes[ 1 ], Is.EqualTo( typeof( TypeSeq1_1 ) ) );
		Assert.That( nodes[ 2 ], Is.EqualTo( typeof( TypeSeq2_1 ) ) );
		Assert.That( nodes[ 3 ], Is.EqualTo( typeof( TypeSeq3_1 ) ) );
	}

	[Test]
	public void AddRemoveThrows() {
		BidirectionalTypeTree<ProcessType> tree = new();

		Assert.Throws<ArgumentNullException>( () => tree.Remove( null! ) );
		Assert.Throws<ArgumentNullException>( () => tree.Add( null! ) );
	}


}
