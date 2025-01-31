using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[UpdateAfter(typeof(CircleOverlapDetectionSystem))]
[BurstCompile]
partial struct ProxyNodeOvelapDetectionSystem : ISystem
{
    EntityQuery query;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
	    query = QueryBuilder().WithAll<SelectedProxyNodeReleasedTag>().Build();
	    state.RequireForUpdate(query);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var proxyNodeEntity = query.GetSingletonEntity();

        if(HasComponent<CollidedTag>(proxyNodeEntity))
        {
            state.EntityManager.DestroyEntity(proxyNodeEntity);
        }
        else
        {
            state.EntityManager.RemoveComponent<SelectedProxyNodeReleasedTag>(query);

            var levelComp = GetSingleton<LevelManagerDataComp>();
            GetComponentRW<LevelDataForProxyNodeComp>(levelComp.curLevelEntity).ValueRW.curProxyNodeCount++;
        }
    }
}
