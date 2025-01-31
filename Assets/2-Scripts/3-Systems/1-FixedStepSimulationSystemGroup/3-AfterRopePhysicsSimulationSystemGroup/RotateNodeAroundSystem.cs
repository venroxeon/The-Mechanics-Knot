//using Unity.Burst;
//using Unity.Entities;
//using Unity.Transforms;
//using static Unity.Entities.SystemAPI;

//[UpdateInGroup(typeof(BeforeRopePhysicsSimulationSystemGroup))]
//[UpdateAfter(typeof(NodeRotationSystem))]
//[BurstCompile]
//partial struct RotateNodeAroundSystem : ISystem
//{
//    EntityQuery query;

//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        query = QueryBuilder().WithAll<LocalTransform, RotateAroundNodeComp, NodeRotatorComp>().Build();

//        state.RequireForUpdate(query);
//    }

//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        RotateNodeAroundJob rotateAroundJob = new()
//        {
//            fixedDeltaTime = GetSingleton<FixedStepSimulationParamComp>().fixedDeltaTime
//        };

//        state.Dependency = rotateAroundJob.ScheduleParallel(query, state.Dependency);
//    }
//}
