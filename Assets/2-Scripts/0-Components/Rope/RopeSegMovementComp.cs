using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct RopeSegMovementComp : IComponentData
{
    public bool hasMovementFromNode, shouldRotateNode;
}
