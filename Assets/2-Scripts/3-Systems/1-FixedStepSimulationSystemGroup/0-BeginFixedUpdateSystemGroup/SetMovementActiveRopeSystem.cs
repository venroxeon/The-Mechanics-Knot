//using Unity.Burst;
//using Unity.Entities;
//using static Unity.Entities.SystemAPI;

//[UpdateInGroup(typeof(BeforeRopePhysicsSimulationSystemGroup))]
//[UpdateAfter(typeof(RopeNodeBasicRotationChainingMechanicSystem))]
//[BurstCompile]
//partial struct SetMovmentActiveRopeSystem : ISystem
//{
//    EntityQuery query;
//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        query = QueryBuilder().WithAll<SetMovementActiveRopeComp, RopeSegEntityBuffer>().Build();
//        state.RequireForUpdate(query);
//    }

//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        SetMovementActiveRopeJob setMoveJob = new()
//        {
//            segCollCompLookup = GetComponentLookup<RopeSegCollisionComp>(true),
//            segMoveCompLookup = GetComponentLookup<RopeSegMovementComp>()
//        };

//        state.Dependency = setMoveJob.ScheduleParallel(query, state.Dependency);
//    }
//}
