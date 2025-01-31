using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
public partial class AfterTransformSystemGroup : ComponentSystemGroup
{
}

[UpdateBefore(typeof(TransformSystemGroup))]
public partial class BeforeTransformSystemGroup : ComponentSystemGroup
{
}

[UpdateInGroup(typeof(AfterTransformSystemGroup))]
public partial class LevelCompletionEvaluationSystemGroup : ComponentSystemGroup
{
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class RopePhysicsSimulationSystemGroup : ComponentSystemGroup
{
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(RopePhysicsSimulationSystemGroup))]
public partial class AfterRopePhysicsSimulationSystemGroup : ComponentSystemGroup
{
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(RopePhysicsSimulationSystemGroup))]
public partial class BeforeRopePhysicsSimulationSystemGroup : ComponentSystemGroup
{
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
public partial class BeginFixedUpdateSystemGroup : ComponentSystemGroup
{
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class InputSystemGroup : ComponentSystemGroup
{
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(InputSystemGroup))]
public partial class SpawnerSystemGroup : ComponentSystemGroup
{
}

[UpdateInGroup(typeof(SpawnerSystemGroup))]
public partial class RopeSpawnerSystemGroup : ComponentSystemGroup
{
}

