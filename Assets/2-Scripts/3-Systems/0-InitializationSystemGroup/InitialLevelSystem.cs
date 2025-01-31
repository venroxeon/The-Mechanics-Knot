using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;
using UnityEngine;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
partial struct InitialLevelSystem : ISystem
{
    EntityQuery query;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        query = QueryBuilder().WithAll<JumpToLevelComp, LevelManagerDataComp, LevelBuffer>().Build();
        state.RequireForUpdate(query);
    }    
    
    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ECB = GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.World.Unmanaged);
        
        foreach(var (_levelManagerComp, levelBuffer) in Query<RefRW<LevelManagerDataComp>, DynamicBuffer<LevelBuffer>>().WithAll<JumpToLevelComp>())
        {
            //GETTER
            var levelManagerComp = _levelManagerComp.ValueRO;
            ///

            int levelIndex = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0);

            var nextLevelEntity = levelBuffer[levelIndex].levelEntity;

            levelManagerComp.curLevelIndex = levelIndex;
            levelManagerComp.curLevelEntity = nextLevelEntity;
        
            for(int i = 0; i < levelIndex; i++)
            {
                ECB.AddComponent<SetEnabledDelayComp>(levelBuffer[i].levelEntity, new()
                {
                    isEnabled = false,
                    delayTime = levelManagerComp.curLevelDisableTime
                });
            }
        
            ECB.AddComponent<SetEnabledDelayComp>(nextLevelEntity, new()
            {
                isEnabled = true,
                delayTime = levelManagerComp.nextLevelEnableTime
            });
        
            SlideCameraTo.Instance.SlideCamera(GetComponent<LocalToWorld>(nextLevelEntity).Position);

            //SETTER
            _levelManagerComp.ValueRW = levelManagerComp;
            ///
        }

        ECB.RemoveComponent<JumpToLevelComp>(query, EntityQueryCaptureMode.AtPlayback);
    }
}
