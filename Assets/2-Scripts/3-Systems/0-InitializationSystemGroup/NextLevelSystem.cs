using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
partial struct NextLevelSystem : ISystem
{
    EntityQuery levelQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        levelQuery = QueryBuilder().WithAll<NextButtonPressedTag, LevelManagerDataComp, LevelBuffer>().Build();

        state.RequireForUpdate(levelQuery);
    }
    
    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var levelEntity = GetSingletonEntity<LevelManagerDataComp>();
        
        var levelManagerComp = GetComponent<LevelManagerDataComp>(levelEntity);
        var levelBuffer = GetBuffer<LevelBuffer>(levelEntity);

        var curLevelEntity = levelManagerComp.curLevelEntity;
        var nextLevelEntity = levelBuffer[levelManagerComp.curLevelIndex + 1].levelEntity;
        
        state.EntityManager.AddComponent<LevelCompleteTag>(curLevelEntity);
        
        state.EntityManager.AddComponentData<SetEnabledDelayComp>(curLevelEntity, new()
        {
            isEnabled = false,
            delayTime = levelManagerComp.curLevelDisableTime
        });
        state.EntityManager.AddComponentData<SetEnabledDelayComp>(nextLevelEntity, new()
        {
            isEnabled = true,
            delayTime = levelManagerComp.nextLevelEnableTime
        });

        levelManagerComp.curLevelEntity = nextLevelEntity;
        levelManagerComp.curLevelIndex++;

        //SET PLAYER PREFABS
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, levelManagerComp.curLevelIndex);
        PlayerPrefs.Save();
        ///

        SetComponent(levelEntity, levelManagerComp);

        state.EntityManager.RemoveComponent<NextButtonPressedTag>(levelEntity);
        
        SlideCameraTo.Instance.SlideCamera(GetComponent<LocalToWorld>(nextLevelEntity).Position);
    }
}
