using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public struct RotateAroundNodeComp : IComponentData
{
    public float rotateAroundSpeed;
    public float3 pivotPos;
    public Entity pivotEntity;
}
