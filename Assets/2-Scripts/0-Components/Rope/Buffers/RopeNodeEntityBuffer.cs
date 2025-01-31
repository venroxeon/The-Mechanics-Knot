using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct RopeNodeEntityBuffer : IBufferElementData
{
    public Entity nodeEntity;
}
