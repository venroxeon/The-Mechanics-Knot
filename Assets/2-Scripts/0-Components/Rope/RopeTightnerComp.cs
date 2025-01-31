using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct RopeTightnerComp : IComponentData
{
    public int removeCount;
    public float extraLenMin, extraLenMax, reelDelayDuration, curTotalTime;
}
