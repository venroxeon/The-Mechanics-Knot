using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct CollidableComp : IComponentData
{
    public bool isTrigger;
    public float radius;
}