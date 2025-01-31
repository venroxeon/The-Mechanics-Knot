using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct InputComp : IComponentData
{
    public bool isFirstTouchPress, isSecTouchPress;
    public float3 firstTouchPos, secTouchPos;
}