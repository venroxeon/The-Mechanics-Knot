using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct NodeRopeEntityBuffer : IBufferElementData
{
    public Entity ropeEntity;
}
