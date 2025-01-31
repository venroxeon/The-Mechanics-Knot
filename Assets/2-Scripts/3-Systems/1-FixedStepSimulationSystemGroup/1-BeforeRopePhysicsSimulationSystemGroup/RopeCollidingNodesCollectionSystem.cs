using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(BeforeRopePhysicsSimulationSystemGroup))]
[UpdateAfter(typeof(RopeSegCollisionSystem))]
[BurstCompile]
partial struct RopeCollidingNodesCollectionSystem : ISystem
{
    EntityQuery ropeNodeBufferQuery, nodeQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ropeNodeBufferQuery = QueryBuilder().WithAll<RopeGenerationCompleteTag, RopeSegEntityBuffer, RopeNodeEntityBuffer>().WithNone<RopeSlicedComp>().Build();

        nodeQuery = QueryBuilder().WithAll<CollidableComp, NodeRotatorComp>().Build();

        state.RequireForUpdate(ropeNodeBufferQuery);
        state.RequireForUpdate(nodeQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var arrRopes = ropeNodeBufferQuery.ToEntityArray(Allocator.TempJob);
        var ropeNodeHashSet = new NativeHashSet<Entity>(nodeQuery.CalculateEntityCount(), Allocator.TempJob);

        RopeCollidingNodesCollectionJob sortJob = new()
        {
            arrRopes = arrRopes,
            ropeNodeHashSet = ropeNodeHashSet,
            segCollCompLookup = GetComponentLookup<RopeSegCollisionComp>(true),
            ropeSegEntityBufferLookup = GetBufferLookup<RopeSegEntityBuffer>(true),
            ropeNodeEntityBufferLookup = GetBufferLookup<RopeNodeEntityBuffer>(),
            nodeRopeEntityBufferLookup = GetBufferLookup<NodeRopeEntityBuffer>(),
        };

        state.Dependency = sortJob.Schedule(arrRopes.Length, state.Dependency);

        arrRopes.Dispose(state.Dependency);
        ropeNodeHashSet.Dispose(state.Dependency);
    }
}
