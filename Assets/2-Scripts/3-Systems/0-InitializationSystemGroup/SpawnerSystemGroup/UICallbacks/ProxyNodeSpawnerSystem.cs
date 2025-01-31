using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(SpawnerSystemGroup))]
[BurstCompile]
partial struct ProxyNodeSpawnerSystem : ISystem
{
    bool isSpawned;
    Entity activeproxyNodeEntity;
    EntityQuery query;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        query = QueryBuilder().WithAll<ProxyNodeButtonPressedComp>().Build();
        state.RequireForUpdate(query);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var proxyNodePressedComp = GetSingleton<ProxyNodeButtonPressedComp>();
        var levelComp = GetSingleton<LevelManagerDataComp>();
        var proxyNodeLevelDataComp = GetComponent<LevelDataForProxyNodeComp>(levelComp.curLevelEntity);

        if (proxyNodeLevelDataComp.curProxyNodeCount < proxyNodeLevelDataComp.maxProxyNodeCount)
        {
            if (!isSpawned && proxyNodePressedComp.isTouchActive)
            {
                var proxyNodePrefComp = GetSingleton<ProxyNodePrefEntityComp>();

                activeproxyNodeEntity = state.EntityManager.Instantiate(proxyNodePrefComp.proxyNodePref);
                GetBuffer<LinkedEntityGroup>(levelComp.curLevelEntity).Add(new()
                {
                    Value = activeproxyNodeEntity
                });
                state.EntityManager.AddComponent<SelectedProxyNodeTag>(activeproxyNodeEntity);

                var pos = proxyNodePressedComp.touchPos;
                pos.z = -1;
                pos.y += 0.6f;
                GetComponentRW<LocalTransform>(activeproxyNodeEntity).ValueRW.Position = pos;

                isSpawned = true;
            }
            else if (isSpawned && proxyNodePressedComp.isTouchActive)
            {
                var pos = proxyNodePressedComp.touchPos;
                pos.z = -1;
                pos.y += 0.6f;
                GetComponentRW<LocalTransform>(activeproxyNodeEntity).ValueRW.Position = pos;
            }
            else if (isSpawned && !proxyNodePressedComp.isTouchActive)
            {
                state.EntityManager.DestroyEntity(query);

                state.EntityManager.RemoveComponent<SelectedProxyNodeTag>(activeproxyNodeEntity);
                state.EntityManager.AddComponent<SelectedProxyNodeReleasedTag>(activeproxyNodeEntity);
                
                isSpawned = false;
            }
        }
        else if (!proxyNodePressedComp.isTouchActive)
        {
            state.EntityManager.DestroyEntity(query);
        }
    }
}
