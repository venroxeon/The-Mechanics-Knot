using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(BeforeRopePhysicsSimulationSystemGroup))]
[BurstCompile]
partial struct RopeSegCollisionSystem : ISystem
{
    EntityQuery segQuery, colQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        segQuery = QueryBuilder().WithAll<RopeSegCollisionComp, TempLocalTransform, RopeSegParEntityComp>().Build();
        colQuery = QueryBuilder().WithAll<CollidableComp, LocalTransform>().Build();

        state.RequireForUpdate(segQuery);
        state.RequireForUpdate(colQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> _arrColEntity = colQuery.ToEntityArray(Allocator.TempJob);

        RopeSegCollisionJob colJob = new()
        {
            arrColEntity = _arrColEntity,
            ropePhysCompLookup = GetComponentLookup<RopePhysComp>(true),
            basicCompLookup = GetComponentLookup<RopeBasicComp>(true),
            trfmLookup = GetComponentLookup<LocalTransform>(true),
            collCompLookup = GetComponentLookup<CollidableComp>(true),
            storageInfoEntity = GetEntityStorageInfoLookup()
        };

        state.Dependency = colJob.ScheduleParallel(segQuery, state.Dependency);

        _arrColEntity.Dispose(state.Dependency);
    }
}
