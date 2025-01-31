using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct KnifeComp : IComponentData
{
    public float3 prevPos;
}
