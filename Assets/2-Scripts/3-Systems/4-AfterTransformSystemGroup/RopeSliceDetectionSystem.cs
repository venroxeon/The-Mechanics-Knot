using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(AfterTransformSystemGroup))]
[BurstCompile]
partial struct RopeSliceDetectionSystem : ISystem
{
    EntityQuery ropeQuery, spikeKnifeQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ropeQuery = QueryBuilder().WithAll<
            RopeGenerationCompleteTag,
            RopeBasicComp,
            RopePhysComp,
            RopeSegEntityBuffer>().WithNone<RopeSlicedComp>().Build();

        spikeKnifeQuery = QueryBuilder().WithAny<SpikeTag, KnifeComp>().Build();

        state.RequireForUpdate(ropeQuery);
        state.RequireForUpdate(spikeKnifeQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> _arrRopeEntities = ropeQuery.ToEntityArray(Allocator.TempJob);

        bool isKnifeActive = false;
        float3 knifePos = default;
        
        if(TryGetSingletonEntity<KnifeComp>(out var knifeEntity))
        {
            knifePos = GetComponent<TempLocalTransform>(knifeEntity).Position;
            isKnifeActive = true;
        }

        RopeSliceDetectionJob sliceDetectionJob = new()
        {
            arrRopeEntities = _arrRopeEntities,
            isKnifeActive = isKnifeActive,
            knifePos = new(knifePos.x, knifePos.y),
            segVelCompLookup = GetComponentLookup<RopeSegVelocityComp>(),
            ropePhysCompLookup = GetComponentLookup<RopePhysComp>(),
            ropeBasicCompLookup = GetComponentLookup<RopeBasicComp>(true),
            ropeSegCollisionCompLookup = GetComponentLookup<RopeSegCollisionComp>(true),
            spikeTagLookup = GetComponentLookup<SpikeToNodeLinkComp>(true),
            tempTrfmCompLookup = GetComponentLookup<TempLocalTransform>(true),
            segParCompLookup = GetComponentLookup<RopeSegParEntityComp>(true),
            segEntityBufferLookup = GetBufferLookup<RopeSegEntityBuffer>(true),
            ECB = GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        };

        state.Dependency = sliceDetectionJob.ScheduleParallel(_arrRopeEntities.Length, (_arrRopeEntities.Length / 5) + 1, state.Dependency);

        _arrRopeEntities.Dispose(state.Dependency);
    }
}
