﻿using Engine.GameLogic.ECPS;

namespace StandardPackage.ECPS.Components;

public sealed class Mass3Component : ComponentBase/*, ISerializable*/ {
	public float Mass { get; set; }
	//public IInertiaTensorProvider InertiaTensorProvider { get; set; }

	protected override string UniqueNameTag => $"{Mass}kg";

	public Mass3Component() {
		Mass = 1;
	}
}

//Get mass and inertia tensor from PhysicsShape3Component using a processor