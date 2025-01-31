using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct RopeGeneratorComp : IComponentData
{
    public float3 lastSegPos;
}