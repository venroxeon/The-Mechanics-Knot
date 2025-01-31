using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct RopeDestroyTimerComp : IComponentData
{
    public bool isDecrementedFromLevelData;
    public float curTotalTime;
}
