using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct RopeSegParEntityComp : IComponentData
{
    public Entity parEntity;
}
