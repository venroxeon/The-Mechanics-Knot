using Unity.Entities;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public struct LevelManagerDataComp : IComponentData
{
    public int curLevelIndex;
    public float curTotalTime;
    public float nextLevelEnableTime, curLevelDisableTime;
    public Entity curLevelEntity;
}
