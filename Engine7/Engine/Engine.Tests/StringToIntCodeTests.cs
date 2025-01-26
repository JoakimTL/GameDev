namespace Engine.Tests;

[TestFixture]
public class StringToIntCodeTests {

	//Test cases:
	//01000001_01000010_01000011_01000100 ("ABCD") = 1094861636
	//10100001_10100010_10100011_10100100 ("¡¢£¤") = 2711790500

	[Test]
	public void TestStringToIntCode() {
		Assert.That( "ABCD".ToIntCode(), Is.EqualTo( unchecked((int) 0b01000001_01000010_01000011_01000100) ) );
		Assert.That( "¡¢£¤".ToIntCode(), Is.EqualTo( unchecked((int) 0b10100001_10100010_10100011_10100100) ) );
	}

}