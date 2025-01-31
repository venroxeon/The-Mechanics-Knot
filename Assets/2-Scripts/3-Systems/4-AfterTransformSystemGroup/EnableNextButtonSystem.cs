using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(AfterTransformSystemGroup))]
[BurstCompile]
partial struct EnableNextButtonSystem : ISystem
{
    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (TryGetSingletonEntity<LevelManagerDataComp>(out var entity))
        {
            var levelManagerComp = GetComponentRW<LevelManagerDataComp>(entity);
            var levelBuffer = GetBuffer<LevelBuffer>(entity);

            var curLevelEntity = levelManagerComp.ValueRO.curLevelEntity;

            if (HasComponent<LevelValidTag>(curLevelEntity))
            {
                levelManagerComp.ValueRW.curTotalTime -= SystemAPI.Time.DeltaTime;

                TimerTextSingleton.Instance.gameObject.SetActive(true);
                TimerTextSingleton.Instance.textMesh.text = levelManagerComp.ValueRO.curTotalTime.ToString("F2");

                if(levelManagerComp.ValueRO.curTotalTime <= 0)
                {
                    TimerTextSingleton.Instance.gameObject.SetActive(false);
                    
                    if(levelManagerComp.ValueRO.curLevelIndex < levelBuffer.Length - 1)
                    {
                        NextLevelButtonSingleton.Instance.gameObject.SetActive(true);
                    }
                    else
                    {
                        NextBatchButtonSingleton.Instance.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                levelManagerComp.ValueRW.curTotalTime = 3;
                TimerTextSingleton.Instance.gameObject.SetActive(false);
                TimerTextSingleton.Instance.textMesh.text = levelManagerComp.ValueRO.curTotalTime.ToString();

                NextLevelButtonSingleton.Instance.gameObject.SetActive(false);
                NextBatchButtonSingleton.Instance.gameObject.SetActive(false);
            }
        }
    }
}
