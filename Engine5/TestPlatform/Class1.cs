﻿using System.Runtime.InteropServices;

namespace TestPlatformClient;
[Test]
[Test<TestAttribute>]
[Test<Class12>]
[Guid( "1ca2bd38-ee5e-49a7-a77f-51920690b86c" )]
internal class Class12 {

}
internal class Class2 {

}
internal class Class3 {

}

[AttributeUsage( AttributeTargets.All, AllowMultiple = true, Inherited = true )]
public class TestAttribute : Attribute {
	public string test = "test";
}
public class TestAttribute<T> : TestAttribute {
	public TestAttribute() {
		this.test = typeof( T ).FullName ?? "none";
	}
}
