using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct RopeSlicedComp : IComponentData
{
    public int sliceIndex;
}
