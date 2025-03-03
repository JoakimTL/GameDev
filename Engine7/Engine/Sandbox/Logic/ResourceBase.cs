﻿using System.Runtime.InteropServices;

namespace Sandbox.Logic;
public abstract class SelfIdentifyingBase {
	public Guid Id { get; }
	protected SelfIdentifyingBase() {
		GuidAttribute guidAttribute = TypeManager.ResolveType( GetType() ).GetAttribute<GuidAttribute>();
		string guidString = guidAttribute.Value;
		this.Id = new( guidString );
	}

	public override string ToString() => GetType().Name;
	public override bool Equals( object? obj ) => obj is SelfIdentifyingBase other && other.Id == Id;
	public override int GetHashCode() => Id.GetHashCode();
}