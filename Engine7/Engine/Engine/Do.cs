namespace Engine;

public static class Do {
	internal interface IProcessDirection {
		/// <summary>
		/// The common process type between this type and the chosen type.
		/// </summary>
		Type ProcessType { get; }
	}
	internal interface IProcessBefore : IProcessDirection {
		/// <summary>
		/// The type to process before.
		/// </summary>
		Type BeforeType { get; }
	}
	internal interface IProcessAfter : IProcessDirection {
		/// <summary>
		/// The type to process after.
		/// </summary>
		Type AfterType { get; }
	}

	/// <summary>
	/// Determines an order of processing for this type and the <see cref="TAfterType"/>.
	/// </summary>
	/// <typeparam name="TAfterType">The type to process after.</typeparam>
	/// <typeparam name="TProcessType">The common process type between this type and the <see cref="TAfterType"/>. Example would be <see cref="IUpdateable"/>, which would order this type after the <see cref="TAfterType"/> in processing whenever <see cref="IUpdateable.Update(double, double)"/> should be executed.</typeparam>
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
	public class AfterAttribute<TAfterType, TProcessType>() : Attribute, IProcessAfter {
		public Type AfterType { get; } = typeof( TAfterType );
		public Type ProcessType { get; } = typeof( TProcessType );
	}

	/// <summary>
	/// Determines an order of processing for this type and the <see cref="TBeforeType"/>.
	/// </summary>
	/// <typeparam name="TBeforeType">The type to process before.</typeparam>
	/// <typeparam name="TProcessType">The common process type between this type and the <see cref="TBeforeType"/>. Example would be <see cref="IUpdateable"/>, which would order this type after the <see cref="TBeforeType"/> in processing whenever <see cref="IUpdateable.Update(double, double)"/> should be executed.</typeparam>
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
	public class BeforeAttribute<TBeforeType, TProcessType>() : Attribute, IProcessBefore {
		public Type BeforeType { get; } = typeof( TBeforeType );
		public Type ProcessType { get; } = typeof( TProcessType );
	}

}
