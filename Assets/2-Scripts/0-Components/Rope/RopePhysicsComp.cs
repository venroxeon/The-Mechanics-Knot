using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct RopePhysComp : IComponentData
{
    public bool applyConst;
    public int iterCount;
    public float3 gravity;
}