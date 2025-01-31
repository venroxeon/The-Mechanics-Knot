using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct RopeSegVelocityComp : IComponentData
{
    public bool hasVelocity;
    public float3 prevPos;
}
