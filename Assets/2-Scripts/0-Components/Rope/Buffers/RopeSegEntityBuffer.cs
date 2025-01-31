using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct RopeSegEntityBuffer : IBufferElementData
{
    public Entity segEntity;
}