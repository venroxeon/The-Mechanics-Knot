using Unity.Entities;
using Unity.Burst;
using System.Runtime.InteropServices;

[BurstCompile]
public struct RopeSegCollisionComp : IComponentData
{
    [MarshalAs(UnmanagedType.U1)] public bool isCol, hasEntered;
    public Entity closestCollEntity;
}
