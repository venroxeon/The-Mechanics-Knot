using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct ProxyNodeButtonPressedComp : IComponentData
{
    public bool isTouchActive;
    public float3 touchPos;
}