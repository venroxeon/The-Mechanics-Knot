using Unity.Burst;
using Unity.Entities;
using static Unity.Entities.SystemAPI;

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
[UpdateInGroup(typeof(PostBakingSystemGroup), OrderLast = true)]
[BurstCompile]
partial struct DisableOnRunBakerSystem : ISystem
{
    EntityQuery toDisableQuery, disabledQuery;
    
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        toDisableQuery = QueryBuilder().WithAll<DisableOnRunTag>().Build();
        disabledQuery = QueryBuilder().WithAll<Disabled, MyDisabledTag>().WithNone<DisableOnRunTag>().Build();

        state.RequireAnyForUpdate(toDisableQuery, disabledQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var arr1 = toDisableQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
        foreach(var entity in arr1)
        {
            state.EntityManager.SetEnabled(entity, false);
            state.EntityManager.AddComponent<MyDisabledTag>(entity);
        }

        var arr2 = disabledQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
        foreach (var entity in arr2)
        {
            state.EntityManager.SetEnabled(entity, true);
            state.EntityManager.RemoveComponent<MyDisabledTag>(entity);
        }

        arr1.Dispose();
        arr2.Dispose();
    }
}
