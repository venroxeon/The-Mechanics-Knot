using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public struct TempLocalTransform : IComponentData
{
    public float Scale;
    public float3 Position;
    public quaternion Rotation;
}