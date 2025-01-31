using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(BeforeRopePhysicsSimulationSystemGroup))]
[UpdateAfter(typeof(RopeCollidingNodesCollectionSystem))]
[UpdateBefore(typeof(NodeBasicRotationBySegmentSystem))]
[BurstCompile]
partial struct RopeNodeBasicRotationChainingMechanicSystem : ISystem
{
    EntityQuery nodeQuery, ropeQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        nodeQuery = QueryBuilder().WithAll<CollidableComp, NodeRotatorComp, NodeRopeEntityBuffer>().Build();
        ropeQuery = QueryBuilder().WithAll<RopeGenerationCompleteTag, RopeBasicComp>().Build();

        state.RequireForUpdate(nodeQuery);
        state.RequireForUpdate(ropeQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var arrNodes = nodeQuery.ToEntityArray(Allocator.TempJob);

        int maxRopeCount = ropeQuery.CalculateEntityCount();
        NativeQueue<Entity> queueMainNodes = new(Allocator.TempJob);
        NativeHashSet<Entity> processedRopes = new(maxRopeCount, Allocator.TempJob);

        RopeNodeBasicRotationChainingMechanicJob basicMechJob = new()
        {
            arrNodes = arrNodes,
            queueMainNodes = queueMainNodes,
            processedRopes = processedRopes,
            segMoveCompLookup = GetComponentLookup<RopeSegMovementComp>(),
            segVelCompLookup = GetComponentLookup<RopeSegVelocityComp>(),
            nodeRotCompLookup = GetComponentLookup<NodeRotatorComp>(),
            segCollCompLookup = GetComponentLookup<RopeSegCollisionComp>(true),
            ropeSegBufferLookup = GetBufferLookup<RopeSegEntityBuffer>(true),
            ropeNodeBufferLookup = GetBufferLookup<RopeNodeEntityBuffer>(true),
            nodeRopeBufferLookup = GetBufferLookup<NodeRopeEntityBuffer>(true)
        };


        state.Dependency = basicMechJob.Schedule(state.Dependency);

        arrNodes.Dispose(state.Dependency);
        queueMainNodes.Dispose(state.Dependency);
        processedRopes.Dispose(state.Dependency);
        //UnityEngine.Debug.Log($"Begin");
        //foreach (var elem in listRopeEntity)
        //{
        //    UnityEngine.Debug.Log(state.EntityManager.GetComponentObject<SpriteRenderer>(elem).name);
        //}
        //UnityEngine.Debug.Log($"End");

        //listRopeEntity.Dispose();
    }
}
