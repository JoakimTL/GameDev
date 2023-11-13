using System.Numerics;

namespace Game.VoxelCitySim.AI.Actions;

public sealed class MoveToAction( Vector3 target ) : ActionBase<Vector3>( target );
