namespace Engine.Processing;

public static class Do<TProcessType> {

	/// <summary>
	/// Determines an order of processing for this type and the <see cref="TAfterType"/>.
	/// </summary>
	/// <typeparam name="TAfterType">The type to process after.</typeparam>
	/// <typeparam name="TProcessType">The common process type between this type and the <see cref="TAfterType"/>. Example would be <see cref="IUpdateable"/>, which would order this type after the <see cref="TAfterType"/> in processing whenever <see cref="IUpdateable.Update(double, double)"/> should be executed.</typeparam>
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
	public class AfterAttribute<TAfterType>() : Attribute, IProcessAfter {
		public Type AfterType { get; } = typeof( TAfterType );
		public Type ProcessType { get; } = typeof( TProcessType );
	}

	/// <summary>
	/// Determines an order of processing for this type and the <see cref="TBeforeType"/>.
	/// </summary>
	/// <typeparam name="TBeforeType">The type to process before.</typeparam>
	/// <typeparam name="TProcessType">The common process type between this type and the <see cref="TBeforeType"/>. Example would be <see cref="IUpdateable"/>, which would order this type after the <see cref="TBeforeType"/> in processing whenever <see cref="IUpdateable.Update(double, double)"/> should be executed.</typeparam>
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
	public class BeforeAttribute<TBeforeType>() : Attribute, IProcessBefore {
		public Type BeforeType { get; } = typeof( TBeforeType );
		public Type ProcessType { get; } = typeof( TProcessType );
	}

	/// <summary>
	/// Attempts to place this type at the end of the processing order. Does not override <see cref="BeforeAttribute{TBeforeType}"/> nor <see cref="AfterAttribute{TAfterType}"/>.
	/// </summary>
	/// <typeparam name="TProcessType">The process type we order for. Example would be <see cref="IUpdateable"/>, which would order this type after the <see cref="TBeforeType"/> in processing whenever <see cref="IUpdateable.Update(double, double)"/> should be executed.</typeparam>
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
	public class LastAttribute() : Attribute, IProcessLast {
		public Type ProcessType { get; } = typeof( TProcessType );
	}

}
