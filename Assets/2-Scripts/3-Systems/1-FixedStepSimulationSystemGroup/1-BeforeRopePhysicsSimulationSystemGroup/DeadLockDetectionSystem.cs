using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(BeforeRopePhysicsSimulationSystemGroup))]
[UpdateAfter(typeof(RopeCollidingNodesCollectionSystem))]
[UpdateAfter(typeof(RopeNodeBasicRotationChainingMechanicSystem))]
[UpdateBefore(typeof(NodeRotationSystem))]
[BurstCompile]
partial struct DeadLockDetectionSystem : ISystem
{
    float deadlockTotalTime, deadlockTimeLimit;
    EntityQuery nodeQuery, ropeQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        deadlockTimeLimit = 2;

        nodeQuery = QueryBuilder().WithAll<CollidableComp, NodeRotatorComp, DeadlockSimulationDataComp>().Build();
        ropeQuery = QueryBuilder().WithAll<RopeGenerationCompleteTag, RopeBasicComp>().Build();

        state.RequireForUpdate(nodeQuery);
        state.RequireForUpdate(ropeQuery);
    }

    public void OnUpdate(ref SystemState state)
    {
        NativeReference<bool> hasDeadlock = new(Allocator.TempJob);
        hasDeadlock.Value = false;

        ScheduleJob(ref hasDeadlock, ref state);

        if(DeadlockCautionSingleton.Instance != null)
        {
            if (hasDeadlock.Value == true)
            {
                deadlockTotalTime += Time.DeltaTime;
                if(deadlockTotalTime > deadlockTimeLimit)
                    DeadlockCautionSingleton.Instance.gameObject.SetActive(true);

                //ADD AND REMOVE DEADLOCK ACTIVE TAG FROM CURRENT LEVEL ENTITY
                var levelManData = GetSingleton<LevelManagerDataComp>();
                state.EntityManager.AddComponent<DeadlockActiveTag>(levelManData.curLevelEntity);

                var ECB = GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
                ECB.RemoveComponent<DeadlockActiveTag>(levelManData.curLevelEntity);
                ///
            }
            else
            {
                DeadlockCautionSingleton.Instance.gameObject.SetActive(false);
                deadlockTotalTime = 0;
            }
        }

        hasDeadlock.Dispose();
    }

    [BurstCompile]
    public void ScheduleJob(ref NativeReference<bool> hasDeadlock, ref SystemState state)
    {
        var arrNodes = nodeQuery.ToEntityArray(Allocator.TempJob);

        int maxRopeCount = ropeQuery.CalculateEntityCount();
        NativeHashSet<Entity> hashSetProcessedRopes = new(maxRopeCount, Allocator.TempJob);
        NativeHashSet<NodeData> hashSetProcessedNodes = new(arrNodes.Length * arrNodes.Length, Allocator.TempJob);
        NativeQueue<NodeData> queueMainNodes = new(Allocator.TempJob);

        DeadLockDetectionJob basicMechJob = new()
        {
            hasDeadlock = hasDeadlock,
            hashSetProcessedRopes = hashSetProcessedRopes,
            hashSetProcessedNodes = hashSetProcessedNodes,
            queueMainNodes = queueMainNodes,
            arrNodes = arrNodes,
            segVelCompLookup = GetComponentLookup<RopeSegVelocityComp>(),
            deadLockSimCompLookup = GetComponentLookup<DeadlockSimulationDataComp>(),
            nodeRotCompLookup = GetComponentLookup<NodeRotatorComp>(),
            trfmLookup = GetComponentLookup<LocalTransform>(true),
            tempTrfmLookup = GetComponentLookup<TempLocalTransform>(true),
            segCollCompLookup = GetComponentLookup<RopeSegCollisionComp>(true),
            ropeSegBufferLookup = GetBufferLookup<RopeSegEntityBuffer>(true),
            ropeNodeBufferLookup = GetBufferLookup<RopeNodeEntityBuffer>(true),
            nodeRopeBufferLookup = GetBufferLookup<NodeRopeEntityBuffer>(true)
        };

        state.Dependency = basicMechJob.Schedule(state.Dependency);

        state.CompleteDependency();

        arrNodes.Dispose();
        hashSetProcessedNodes.Dispose();
        hashSetProcessedRopes.Dispose();
        queueMainNodes.Dispose();
    }
}
