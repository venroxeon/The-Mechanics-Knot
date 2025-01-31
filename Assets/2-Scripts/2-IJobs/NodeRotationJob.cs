using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct NodeRotationJob : IJobEntity
{
    public float fixedDeltaTime;

    [BurstCompile]
    public void Execute(ref NodeRotatorComp nodeRotComp, ref LocalTransform trfm, in CollidableComp collComp, in DeadlockSimulationDataComp deadLockComp)
    {
        float angle = 0;
        float3 axis = new(0, 0, 1);

        if (nodeRotComp.type == CollType.Main)
        {
            if (!nodeRotComp.isInDeadLock)
                angle = nodeRotComp.rotAngleUnscaled * fixedDeltaTime;
            else
                angle = 0;
        }
        else if (nodeRotComp.type == CollType.Default)
        {
            if (nodeRotComp.segPolledColCount > 0)
            {
                angle = nodeRotComp.segPolledRotTotal / nodeRotComp.segPolledColCount;
                //angle *= deadLockComp.assignedDir;
            }
        }

        nodeRotComp.rotAngleCurFrame = angle;

        quaternion quat = quaternion.AxisAngle(axis, angle);
        trfm.Rotation = math.mul(trfm.Rotation, quat);
    }
}
