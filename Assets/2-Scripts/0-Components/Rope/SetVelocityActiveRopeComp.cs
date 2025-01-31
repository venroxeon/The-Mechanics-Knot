using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct SetVelocityActiveRopeComp : IComponentData
{
    public bool isActive;
}
