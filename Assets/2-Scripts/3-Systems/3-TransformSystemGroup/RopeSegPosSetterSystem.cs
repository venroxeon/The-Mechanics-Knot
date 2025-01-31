// using Unity.Entities;
// using Unity.Burst;
// using Unity.Transforms;

// [BurstCompile]
// [UpdateInGroup(typeof(TransformSystemGroup), OrderFirst = true)]
// public partial struct RopeSegPosSetterSystem : ISystem
// {
//     EntityQuery segQuery;

//     [BurstCompile]
//     public void OnCreate(ref SystemState state)
//     {
//         segQuery = SystemAPI.QueryBuilder().WithAll<
//             RopeGenerationCompleteTag,
//             TempLocalTransform,
//             LocalTransform>().Build();
//         state.RequireForUpdate(segQuery);
//     }

//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         RopeSegSetterJob job = new();

//         state.Dependency = job.ScheduleParallel(segQuery, state.Dependency);
//     }
// }
