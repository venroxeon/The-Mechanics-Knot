using Unity.Entities;
using Unity.Burst;

public enum CollType
{
    Default,
    Main
}


[BurstCompile]
public struct NodeRotatorComp : IComponentData
{
    public bool isRotating, isInDeadLock;
    public int segPolledColCount;
    public float rotAngleCurFrame, rotAngleUnscaled, segPolledRotTotal;

    public CollType type;
}