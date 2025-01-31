using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct SpikeToNodeLinkComp : IComponentData
{
    public bool isActive;
    public Entity linkedNode;
}
