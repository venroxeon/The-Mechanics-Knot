using Unity.Burst;
using Unity.Entities;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(AfterTransformSystemGroup))]
[BurstCompile]
partial struct SlicedRopeDestroyDelaySystem : ISystem
{
    EntityQuery query1, query2;

    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        query1 = QueryBuilder().WithAll<RopeSlicedComp>().WithNone<RopeDestroyTimerComp>().Build();
        query2 = QueryBuilder().WithAll<RopeSlicedComp, RopeDestroyTimerComp>().Build();

        state.RequireAnyForUpdate(query1, query2);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ECB = new(Unity.Collections.Allocator.Temp);

        state.EntityManager.AddComponent<RopeDestroyTimerComp>(query1);

        var levelDataComp = GetComponentRW<LevelDataComp>(GetSingleton<LevelManagerDataComp>().curLevelEntity);
        foreach (var (_timeComp, ropeEntity) in Query<RefRW<RopeDestroyTimerComp>>().WithEntityAccess())
        {
            //GETTER
            var timeComp = _timeComp.ValueRO;
            ///

            timeComp.curTotalTime += Time.DeltaTime;

            if(timeComp.curTotalTime > 5)
            {
                ECB.AddComponent<DestroyRopeWithoutDecrementTag>(ropeEntity);
                ECB.RemoveComponent<RopeDestroyTimerComp>(ropeEntity);
            }
            else if(!timeComp.isDecrementedFromLevelData)
            {
                levelDataComp.ValueRW.curRopeCount--;
                timeComp.isDecrementedFromLevelData = true;
            }

            //SETTER
            _timeComp.ValueRW = timeComp;
        }

        if (!ECB.IsEmpty)
            ECB.Playback(state.EntityManager);

        ECB.Dispose();
    }
}
