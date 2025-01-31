using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(BeforeTransformSystemGroup))]
partial struct RopeTightnerSystem : ISystem
{
    EntityQuery ropeQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ropeQuery = SystemAPI.QueryBuilder().WithAll<
            RopeGenerationCompleteTag,
            RopeTightnerComp, 
            RopeBasicComp, 
            RopeSegEntityBuffer>().WithNone<RopeSlicedComp>().Build();

        state.RequireForUpdate(ropeQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        var arrRopes = ropeQuery.ToEntityArray(Allocator.TempJob);

        RopeTightnerJob tightJob = new()
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            arrRopes = arrRopes,
            ropeTightnerCompLookup = SystemAPI.GetComponentLookup<RopeTightnerComp>(),
            segBufferLookup = SystemAPI.GetBufferLookup<RopeSegEntityBuffer>(),
            tempTrfmLookup = SystemAPI.GetComponentLookup<TempLocalTransform>(true),
            segCollCompLookup = SystemAPI.GetComponentLookup<RopeSegCollisionComp>(true),
            ropeBasicCompLookup = SystemAPI.GetComponentLookup<RopeBasicComp>(true),
            ECB = ECB.AsParallelWriter()
        };

        state.Dependency = tightJob.ScheduleParallel(arrRopes.Length, (arrRopes.Length / 5 + 1), state.Dependency);

        arrRopes.Dispose(state.Dependency);
    }
}
