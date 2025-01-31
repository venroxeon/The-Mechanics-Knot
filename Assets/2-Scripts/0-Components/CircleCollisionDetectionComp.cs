using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct CircleCollisionDetectionComp : IComponentData
{
    public float radius;
}
