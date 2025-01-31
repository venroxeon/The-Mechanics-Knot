using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct RotateNodeAroundJob : IJobEntity
{
    public float fixedDeltaTime;

    [BurstCompile]
    public void Execute(ref LocalTransform _Trfm, in NodeRotatorComp nodeRotComp, in RotateAroundNodeComp rotAroundComp)
    {
        float rotDir = math.sign(nodeRotComp.rotAngleCurFrame);
        if (rotDir == 0)
            return;

        float rotSpeed = rotAroundComp.rotateAroundSpeed * rotDir * fixedDeltaTime;
        float3 dirVec = _Trfm.Position - rotAroundComp.pivotPos;
        
        float3 newPos = math.rotate(quaternion.AxisAngle(math.forward(), rotSpeed), dirVec);

        _Trfm.Position = newPos + rotAroundComp.pivotPos;
    }
}
