using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public partial struct RopSegReset_NoCollisionJob : IJobEntity
{
    [BurstCompile]
    public void Execute(ref RopeSegVelocityComp velComp, ref RopeSegMovementComp moveComp)
    {
        velComp.hasVelocity = true;
        moveComp.hasMovementFromNode = false;
        moveComp.shouldRotateNode = false;
    }
}
