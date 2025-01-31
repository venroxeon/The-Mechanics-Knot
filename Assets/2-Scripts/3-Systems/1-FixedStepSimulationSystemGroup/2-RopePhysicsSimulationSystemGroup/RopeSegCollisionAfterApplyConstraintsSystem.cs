//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Transforms;
//using static Unity.Entities.SystemAPI;

//[UpdateInGroup(typeof(RopePhysicsSimulationSystemGroup))]
//[UpdateAfter(typeof(RopeApplyConstraintsSystem))]
//[BurstCompile]
//partial struct RopeSegCollisionAfterApplyConstraintsSystem : ISystem
//{
//    EntityQuery segQuery, colQuery;

//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        segQuery = QueryBuilder().WithAll<RopeSegCollisionComp, RopeSegVelocityComp, RopeSegParEntityComp>().Build();
//        colQuery = QueryBuilder().WithAll<CollidableComp, LocalTransform>().Build();

//        state.RequireForUpdate(segQuery);
//        state.RequireForUpdate(colQuery);
//    }

//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        NativeArray<Entity> _arrColEntity = colQuery.ToEntityArray(Allocator.TempJob);

//        var trfmLookup = GetComponentLookup<LocalTransform>(true);
//        var collCompLookup = GetComponentLookup<CollidableComp>(true);

//        RopeSegCollisionJob colJob = new()
//        {
//            basicCompLookup = GetComponentLookup<RopeBasicComp>(true),
//            arrColEntity = _arrColEntity,
//            trfmLookup = trfmLookup,
//            collCompLookup = collCompLookup
//        };

//        state.Dependency = colJob.ScheduleParallel(segQuery, state.Dependency);

//        _arrColEntity.Dispose(state.Dependency);
//    }
//}