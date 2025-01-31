using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(RopePhysicsSimulationSystemGroup))]
[BurstCompile]
public partial struct RopeApplyConstraintsSystem : ISystem
{
    EntityQuery ropeQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ropeQuery = QueryBuilder().WithAll<
            RopeGenerationCompleteTag, 
            RopeBasicComp,
            RopePhysComp,
            RopeSegEntityBuffer>().WithNone<RopeSlicedComp>().Build();

        state.RequireForUpdate(ropeQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> _arrRopeEntities = ropeQuery.ToEntityArray(Allocator.TempJob);

        RopeApplyConstraintsJob applyConstJob = new()
        {
            arrRopeEntities = _arrRopeEntities,
            tempTrfmLookup = GetComponentLookup<TempLocalTransform>(),
            segCollCompLookup = GetComponentLookup<RopeSegCollisionComp>(),
            trfmLookup = GetComponentLookup<LocalTransform>(true),
            collCompLookup = GetComponentLookup<CollidableComp>(true),
            ropeBasicCompLookup = GetComponentLookup<RopeBasicComp>(true),
            ropePhysCompLookup = GetComponentLookup<RopePhysComp>(true),
            segEntityBufferLookup = GetBufferLookup<RopeSegEntityBuffer>(true)
        };

        state.Dependency = applyConstJob.ScheduleParallel(_arrRopeEntities.Length, (_arrRopeEntities.Length / 4) + 1, state.Dependency);

        _arrRopeEntities.Dispose(state.Dependency);
    }
}