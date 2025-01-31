using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using static Unity.Entities.SystemAPI;
using Unity.Transforms;

[UpdateInGroup(typeof(RopeSpawnerSystemGroup))]
//[BurstCompile]
public partial struct RopeSpawnerSystem : ISystem
{
    bool isTouchUsed;

    EntityQuery levelQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InputComp>();
        
        levelQuery = QueryBuilder().WithAll<LevelManagerDataComp, LevelBuffer>().Build();
        state.RequireForUpdate(levelQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        InputComp input = SystemAPI.GetSingleton<InputComp>();

        Spawn(input.isFirstTouchPress, input.firstTouchPos, ref state);
    }

    [BurstCompile]
    void Spawn(bool isFirstTouchPress, float3 touchPos, ref SystemState state)
    {
        var levelManager = GetSingleton<LevelManagerDataComp>();
        var curLevelData = GetComponent<LevelDataComp>(levelManager.curLevelEntity);
        
        if(curLevelData.curRopeCount >= curLevelData.ropeCountLimit) return;

        EntityCommandBuffer ECB = new(Unity.Collections.Allocator.TempJob);

        if (isFirstTouchPress && !isTouchUsed)
        {
            isTouchUsed = true;
            var linkedBuffer = GetBuffer<LinkedEntityGroup>(levelManager.curLevelEntity);

            foreach (var (ropeSpawnerComp, _Entity) in SystemAPI.Query<RefRO<RopeSpawnerComp>>().WithEntityAccess())
            {
                Entity ropeEntity = state.EntityManager.Instantiate(ropeSpawnerComp.ValueRO.ropePref);

                ECB.AddComponent<Parent>(ropeEntity, new()
                {
                    Value = _Entity
                });

                SetComponent(ropeEntity, new RopeGeneratorComp() { lastSegPos = touchPos });

                linkedBuffer.Add(ropeEntity);
            }
        }
        else if (isTouchUsed && !isFirstTouchPress)
        {
            isTouchUsed = false;
        }

        if (!ECB.IsEmpty)
            ECB.Playback(state.EntityManager);
        ECB.Dispose();
    }
}