using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(RopePhysicsSimulationSystemGroup))]
[UpdateBefore(typeof(RopeApplyConstraintsSystem))]
[BurstCompile]
partial struct RopeSegMovementSystem : ISystem
{
    float3 axis;
    EntityQuery segQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        segQuery = SystemAPI.QueryBuilder().WithAll<
            RopeSegMovementComp,
            RopeGenerationCompleteTag,
            TempLocalTransform, RopeSegCollisionComp>().Build();

        state.RequireForUpdate(segQuery);
        state.RequireForUpdate<NodeRotatorComp>();

        axis = new(0, 0, 1);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if(SystemAPI.TryGetSingleton<FixedStepSimulationParamComp>(out var fixedTimeComp))
        {
            RopeSegMovementJob segMovementJob = new()
            {
                trfmLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
                nodeRotCompLookup = SystemAPI.GetComponentLookup<NodeRotatorComp>(true)
            };

            state.Dependency = segMovementJob.ScheduleParallel(segQuery, state.Dependency);
        }
    }
}
