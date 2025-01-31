using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(BeforeRopePhysicsSimulationSystemGroup))]
[UpdateBefore(typeof(NodeRotationSystem))]
[BurstCompile]
partial struct NodeBasicRotationBySegmentSystem : ISystem
{
    EntityQuery ropeSegQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ropeSegQuery = QueryBuilder().WithAll<
            RopeGenerationCompleteTag,
            TempLocalTransform, RopeSegVelocityComp, RopeSegMovementComp, RopeSegCollisionComp>().Build();

        state.RequireForUpdate(ropeSegQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NodeBasicRotationBySegmentJob rotJob = new()
        {
            nodeRotCompLookup = GetComponentLookup<NodeRotatorComp>(),
            trfmLookup = GetComponentLookup<LocalTransform>(true)
        };

        state.Dependency = rotJob.Schedule(ropeSegQuery, state.Dependency);
    }
}