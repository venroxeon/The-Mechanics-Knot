using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct SetEnabledDelayComp : IComponentData
{
    public bool isEnabled;
    public float delayTime, curTotalTime;
}
