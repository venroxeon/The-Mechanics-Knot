using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(SpawnerSystemGroup))]
[BurstCompile]
partial struct RopeDestroyerSystem : ISystem
{
    EntityQuery query1, query2, query3;
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        query1 = QueryBuilder().WithAll<LineRendererCompanionLinkComp, RopeGenerationCompleteTag>().Build();
        query2 = QueryBuilder().WithAll<LineRendererCompanionLinkComp, RopeGenerationCompleteTag, DestroyRopeWithDecrementTag>().Build();
        query3 = QueryBuilder().WithAll<LineRendererCompanionLinkComp, DestroyRopeWithoutDecrementTag>().Build();
        
        state.RequireAnyForUpdate(query1, query2, query3);
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ECB = new(Unity.Collections.Allocator.Temp);

        var levelManager = GetSingleton<LevelManagerDataComp>();
        var curLevelData = GetComponent<LevelDataComp>(levelManager.curLevelEntity);

        foreach (var (ropeLineRen, segBuffer, entity) in Query<RefRO<LineRendererCompanionLinkComp>, DynamicBuffer<RopeSegEntityBuffer>>().WithAll<RopeGenerationCompleteTag>().WithEntityAccess())
        {
            if(segBuffer.Length <= 3)
            {
                if (ropeLineRen.ValueRO.lineRen != null)
                {
                    Object.Destroy(ropeLineRen.ValueRO.lineRen.Value.gameObject);
                }

                ECB.DestroyEntity(entity);
                GetComponentRW<LevelDataComp>(levelManager.curLevelEntity).ValueRW.curRopeCount--;
            }
        }

        foreach (var (ropeLineRen, entity) in Query<RefRO<LineRendererCompanionLinkComp>>().WithAll<DestroyRopeWithDecrementTag, RopeGenerationCompleteTag>().WithEntityAccess())
        {
            if(ropeLineRen.ValueRO.lineRen != null)
            {
                Object.Destroy(ropeLineRen.ValueRO.lineRen.Value.gameObject);
            }

            ECB.DestroyEntity(entity);
            GetComponentRW<LevelDataComp>(levelManager.curLevelEntity).ValueRW.curRopeCount--;
        }

        foreach (var (ropeLineRen, entity) in Query<RefRO<LineRendererCompanionLinkComp>>().WithAll<DestroyRopeWithoutDecrementTag>().WithEntityAccess())
        {
            if (ropeLineRen.ValueRO.lineRen != null)
            {
                Object.Destroy(ropeLineRen.ValueRO.lineRen.Value.gameObject);
            }

            ECB.DestroyEntity(entity);
        }

        if (!ECB.IsEmpty)
            ECB.Playback(state.EntityManager);

        ECB.Dispose();
    }
}
