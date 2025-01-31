using Unity.Burst;
using Unity.Entities;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(AfterTransformSystemGroup))]
[BurstCompile]
partial struct SetEnabledDelaySystem : ISystem
{
    EntityQuery setEnabledQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        setEnabledQuery = QueryBuilder().WithAll<SetEnabledDelayComp, LinkedEntityGroup>().WithOptions(EntityQueryOptions.IncludeDisabledEntities).Build();
        
        state.RequireForUpdate(setEnabledQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SetEnabledDelayJob delayJob = new()
        {
            DeltaTime = Time.DeltaTime,
            storageLookup = GetEntityStorageInfoLookup(),
            ECB = GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged)
        };
        state.Dependency = delayJob.Schedule(setEnabledQuery, state.Dependency);
    }
}
