using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public struct FixedStepSimulationParamComp : IComponentData
{
    public float fixedDeltaTime;
}