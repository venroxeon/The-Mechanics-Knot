//using Unity.Entities;
//using Unity.Burst;
//using Unity.Transforms;

//[BurstCompile]
//[UpdateInGroup(typeof(RopePhysicsSimulationSystemGroup), OrderFirst = true)]
//[UpdateBefore(typeof(TransformSystemGroup))]
//public partial struct RopeSegPosGetterSystem : ISystem
//{
//    EntityQuery segQuery;

//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        segQuery = SystemAPI.QueryBuilder().WithAll<
//            RopeGenerationCompleteTag,
//            RopeSegPhysComp, 
//            LocalTransform>().Build();
//        state.RequireForUpdate(segQuery);
//    }

//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        RopeSegGetterJob job = new();

//        state.Dependency = job.ScheduleParallel(segQuery, state.Dependency);
//    }
//}
