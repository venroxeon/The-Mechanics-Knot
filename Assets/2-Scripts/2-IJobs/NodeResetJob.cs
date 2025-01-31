using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public partial struct NodeRotCompResetJob : IJobEntity
{
    [BurstCompile]
    public void Execute(ref NodeRotatorComp nodeRotComp, ref CollidableComp collComp)
    {
        if (nodeRotComp.type == CollType.Main)
        {
            nodeRotComp.isRotating = true;
            nodeRotComp.isInDeadLock = false;
        }
        else if (nodeRotComp.isRotating)
        {
            nodeRotComp.rotAngleUnscaled = 0;
            nodeRotComp.segPolledColCount = 0;
            nodeRotComp.segPolledRotTotal = 0;
            nodeRotComp.isRotating = false;
        }
    }
}

[BurstCompile]
public partial struct NodeBufferResetJob : IJobEntity
{
    [BurstCompile]
    public void Execute(ref DynamicBuffer<NodeRopeEntityBuffer> ropeBuffer)
    {
        ropeBuffer.Clear();
    }
}
