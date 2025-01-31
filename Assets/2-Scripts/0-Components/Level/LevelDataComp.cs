using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct LevelDataComp : IComponentData
{
    public int ropeCountLimit, curRopeCount;
}
