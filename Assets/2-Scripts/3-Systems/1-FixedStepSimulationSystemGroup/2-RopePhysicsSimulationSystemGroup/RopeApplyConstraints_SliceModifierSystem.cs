using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(RopePhysicsSimulationSystemGroup))]
[BurstCompile]
partial struct RopeApplyConstraints_SliceModifierSystem : ISystem
{
    EntityQuery ropeQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ropeQuery = QueryBuilder().WithAll<
            RopeGenerationCompleteTag,
            RopeBasicComp,
            RopePhysComp,
            RopeSlicedComp,
            RopeSegEntityBuffer>().Build();

        state.RequireForUpdate(ropeQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> _arrRopeEntities = ropeQuery.ToEntityArray(Allocator.TempJob);

        RopeApplyConstraints_SliceModifierJob applyConstJob = new()
        {
            arrRopeEntities = _arrRopeEntities,
            tempTrfmLookup = GetComponentLookup<TempLocalTransform>(),
            ropeBasicCompLookup = GetComponentLookup<RopeBasicComp>(true),
            ropePhysCompLookup = GetComponentLookup<RopePhysComp>(true),
            segEntityBufferLookup = GetBufferLookup<RopeSegEntityBuffer>(true),
            sliceCompLookup = GetComponentLookup<RopeSlicedComp>(true)
        };

        state.Dependency = applyConstJob.ScheduleParallel(_arrRopeEntities.Length, (_arrRopeEntities.Length / 4) + 1, state.Dependency);

        _arrRopeEntities.Dispose(state.Dependency);
    }
}
