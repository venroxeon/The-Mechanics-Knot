using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public partial struct RopeSegResetJob : IJobEntity
{
    [BurstCompile]
    public void Execute(in RopeSegCollisionComp segColComp, ref RopeSegVelocityComp velComp, ref RopeSegMovementComp moveComp)
    {
        velComp.hasVelocity = !segColComp.isCol;
        moveComp.hasMovementFromNode = false;
        moveComp.shouldRotateNode = false;
    }
}
