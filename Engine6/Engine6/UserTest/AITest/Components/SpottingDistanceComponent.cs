using Engine.Modules.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserTest.AITest.Components;
/// <summary>
/// Determines the range at which this entity can be spotted. Used for the AI to locate entities. It is assumed that spotting means determination of the entity type as well.
/// </summary>
public sealed class SpottingDistanceComponent : ComponentBase {
	public double SpottingDistance { get; set; }
}
/// <summary>
/// The range at which this entity can spot other entities.
/// </summary>
public sealed class SpottingRangeComponent : ComponentBase {
	public double SpottingRange { get; set; }
}
