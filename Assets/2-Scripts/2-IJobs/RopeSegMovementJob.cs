using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct RopeSegMovementJob : IJobEntity
{
    float3 axis;

    [ReadOnly] public ComponentLookup<NodeRotatorComp> nodeRotCompLookup;
    [ReadOnly] public ComponentLookup<LocalTransform> trfmLookup;

    [BurstCompile]
    public void Execute(ref TempLocalTransform tempTrfm, in RopeSegMovementComp segMoveComp, in RopeSegCollisionComp segCollComp)
    {
        if (!segMoveComp.hasMovementFromNode)
            return;

        axis = new(0, 0, 1);

        NodeRotatorComp nodeRotComp = nodeRotCompLookup[segCollComp.closestCollEntity];

        LocalTransform colTrfm = trfmLookup[segCollComp.closestCollEntity];
        float3 segPos = new float3(tempTrfm.Position.x, tempTrfm.Position.y, colTrfm.Position.z);

        float3 direction = segPos - colTrfm.Position;

        quaternion rotation = quaternion.AxisAngle(axis, nodeRotComp.rotAngleCurFrame);

        float3 newDir = math.rotate(rotation, direction);

        float3 pos = colTrfm.Position + newDir;
        
        tempTrfm.Position.x = pos.x;
        tempTrfm.Position.y = pos.y;
    }
}
