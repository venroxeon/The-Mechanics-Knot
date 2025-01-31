using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(BeforeRopePhysicsSimulationSystemGroup))]
[BurstCompile]
partial struct NodeRotationSystem : ISystem
{
    EntityQuery nodeRotQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        nodeRotQuery = QueryBuilder().WithAll<NodeRotatorComp, CollidableComp, LocalTransform, DeadlockSimulationDataComp>().Build();

        state.RequireForUpdate(nodeRotQuery);
        state.RequireForUpdate<FixedStepSimulationParamComp>();
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if(TryGetSingleton<FixedStepSimulationParamComp>(out var fixedStepComp))
        {
            NodeRotationJob nodeRotJob = new()
            {
                fixedDeltaTime = fixedStepComp.fixedDeltaTime
            };

            state.Dependency = nodeRotJob.ScheduleParallel(nodeRotQuery, state.Dependency);
        }
    }
}
