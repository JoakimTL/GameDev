namespace Engine.Tests;

[TestFixture]
public class TypeManagerTests {

	private abstract class BaseType;

	private class Implementation : BaseType { }
	private class Implementation2 : BaseType { }

	private abstract class FirstLayerBaseTypeGeneric<T>;

	private abstract class SecondLayerBaseTypeGeneric<T> : FirstLayerBaseTypeGeneric<T>;

	private class ImplementationFirstLayerGeneric1 : FirstLayerBaseTypeGeneric<int> { }
	private class ImplementationFirstLayerGeneric2 : FirstLayerBaseTypeGeneric<string> { }

	private class ImplementationSecondLayerGeneric1 : SecondLayerBaseTypeGeneric<int> { }
	private class ImplementationSecondLayerGeneric2 : SecondLayerBaseTypeGeneric<string> { }

	[Test]
	public void TestNonGenericSubclasses() {
		IEnumerable<Type> types = TypeManager.GetAllSubclassesOfGenericType( typeof( BaseType ) );
		CollectionAssert.AreEqual( new[] { typeof( Implementation ), typeof( Implementation2 ) }, types );
	}

	[Test]
	public void TestGenericSubclasses() {
		IEnumerable<Type> types = TypeManager.GetAllSubclassesOfGenericType( typeof( FirstLayerBaseTypeGeneric<> ) );
		CollectionAssert.AreEqual( new[] { typeof( ImplementationFirstLayerGeneric1 ), typeof( ImplementationFirstLayerGeneric2 ), typeof( ImplementationSecondLayerGeneric1 ), typeof( ImplementationSecondLayerGeneric2 ) }, types );

		types = TypeManager.GetAllSubclassesOfGenericType( typeof( SecondLayerBaseTypeGeneric<> ) );
		CollectionAssert.AreEqual( new[] { typeof( ImplementationSecondLayerGeneric1 ), typeof( ImplementationSecondLayerGeneric2 ) }, types );
	}

}