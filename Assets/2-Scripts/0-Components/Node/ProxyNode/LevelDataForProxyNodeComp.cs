using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct LevelDataForProxyNodeComp : IComponentData
{
    public int maxProxyNodeCount;
    public int curProxyNodeCount;
}
