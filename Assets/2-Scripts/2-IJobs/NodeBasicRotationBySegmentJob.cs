using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct NodeBasicRotationBySegmentJob : IJobEntity
{
    public ComponentLookup<NodeRotatorComp> nodeRotCompLookup;
    [ReadOnly] public ComponentLookup<LocalTransform> trfmLookup;

    [BurstCompile]
    public void Execute(in TempLocalTransform trfm, in RopeSegVelocityComp velComp, in RopeSegCollisionComp segCollComp, RopeSegMovementComp segMoveComp)
    {
        if (!segMoveComp.shouldRotateNode || !segCollComp.isCol)
            return;

        NodeRotatorComp nodeRotComp = nodeRotCompLookup[segCollComp.closestCollEntity];

        float3 colPos = trfmLookup[segCollComp.closestCollEntity].Position;

        float2 firstDir = new float2(velComp.prevPos.x - colPos.x, velComp.prevPos.y - colPos.y);
        float2 secDir = new float2(trfm.Position.x - colPos.x, trfm.Position.y - colPos.y);

        float angle = GetAngle(firstDir, secDir);

        if (angle != 0)
        {
            nodeRotComp.segPolledRotTotal += angle;
            nodeRotComp.segPolledColCount++;

            nodeRotCompLookup[segCollComp.closestCollEntity] = nodeRotComp;
        }
    }

    [BurstCompile]
    public static float GetAngle(in float2 firstDir, in float2 secDir)
    {
        float dotProduct = math.dot(math.normalize(firstDir), math.normalize(secDir));
        float angle = math.acos(math.clamp(dotProduct, -1.0f, 1.0f));

        float sign = math.sign(firstDir.x * secDir.y - firstDir.y * secDir.x);
        angle *= sign;

        return angle;
    }
}
