using Unity.Mathematics;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Jobs;
using CustomUtilities;

[BurstCompile]
public struct RopeApplyConstraintsJob : IJobFor
{
    [NativeDisableParallelForRestriction]
    public ComponentLookup<TempLocalTransform> tempTrfmLookup;
    [NativeDisableParallelForRestriction]
    public ComponentLookup<RopeSegCollisionComp> segCollCompLookup;

    [ReadOnly] public ComponentLookup<CollidableComp> collCompLookup;
    [ReadOnly] public ComponentLookup<LocalTransform> trfmLookup;
    [ReadOnly] public ComponentLookup<RopeBasicComp> ropeBasicCompLookup;
    [ReadOnly] public ComponentLookup<RopePhysComp> ropePhysCompLookup;

    [ReadOnly] public BufferLookup<RopeSegEntityBuffer> segEntityBufferLookup;

    [ReadOnly]
    public NativeArray<Entity> arrRopeEntities;

    [BurstCompile]
    public void Execute(int index)
    {
        Entity ropeEntity = arrRopeEntities[index];
        RopeBasicComp ropeBasicComp = ropeBasicCompLookup.GetRefRO(ropeEntity).ValueRO;
        RopePhysComp ropePhysComp = ropePhysCompLookup.GetRefRO(ropeEntity).ValueRO;

        DynamicBuffer<RopeSegEntityBuffer> arrSegEntity = segEntityBufferLookup[ropeEntity];

        if (!ropePhysComp.applyConst)
            return;

        if (arrSegEntity.Length <= 1)
            return;

        for (int k = 0; k < ropePhysComp.iterCount; k++)
        {
            //ApplyConstraints(ropeBasicComp.segDist, ropeBasicComp.segRadius, arrSegEntity);
            ApplyConstraintsAngular(ropeBasicComp.segDist, arrSegEntity, ropeBasicComp.segDistSqr, ropeBasicComp.segRadius);
        }
    }

    [BurstCompile]
    public void ApplyConstraints(in float segDist, in float segRadius, in DynamicBuffer<RopeSegEntityBuffer> arrSegEntity)
    {
        TempLocalTransform firstTempTrfm, secTempTrfm;
        RopeSegCollisionComp firstCol, secCol;
        for (int i = 0; i < (arrSegEntity.Length - 1); i++)
        {
            firstTempTrfm = tempTrfmLookup[arrSegEntity[i].segEntity];
            secTempTrfm = tempTrfmLookup[arrSegEntity[i + 1].segEntity];

            firstCol = segCollCompLookup[arrSegEntity[i].segEntity];
            secCol = segCollCompLookup[arrSegEntity[i + 1].segEntity];

            ApplyDeltaLinear(segDist, segRadius, ref firstTempTrfm, ref secTempTrfm, ref firstCol, ref secCol);

            tempTrfmLookup[arrSegEntity[i].segEntity] = firstTempTrfm;
            tempTrfmLookup[arrSegEntity[i + 1].segEntity] = secTempTrfm;

            segCollCompLookup[arrSegEntity[i].segEntity] = firstCol;
            segCollCompLookup[arrSegEntity[i + 1].segEntity] = secCol;
        }

        firstTempTrfm = tempTrfmLookup[arrSegEntity[^1].segEntity];
        secTempTrfm = tempTrfmLookup[arrSegEntity[0].segEntity];

        firstCol = segCollCompLookup[arrSegEntity[^1].segEntity];
        secCol = segCollCompLookup[arrSegEntity[0].segEntity];

        ApplyDeltaLinear(segDist, segRadius, ref firstTempTrfm, ref secTempTrfm, ref firstCol, ref secCol);

        tempTrfmLookup[arrSegEntity[^1].segEntity] = firstTempTrfm;
        tempTrfmLookup[arrSegEntity[0].segEntity] = secTempTrfm;

        segCollCompLookup[arrSegEntity[^1].segEntity] = firstCol;
        segCollCompLookup[arrSegEntity[0].segEntity] = secCol;
    }

    [BurstCompile]
    public void ApplyDeltaLinear(in float segDist, in float segRadius, ref TempLocalTransform firstTempTrfm, ref TempLocalTransform secTempTrfm, ref RopeSegCollisionComp firstCol, ref RopeSegCollisionComp secCol)
    {
        float dist, error;
        float3 delta;

        delta = secTempTrfm.Position - firstTempTrfm.Position;
        dist = math.distance(secTempTrfm.Position, firstTempTrfm.Position);
        delta /= dist;

        error = dist - segDist;

        delta *= error / 2;

        firstTempTrfm.Position += delta;
        secTempTrfm.Position -= delta;

        CollidableComp colComp = collCompLookup[firstCol.closestCollEntity];
        LocalTransform colTrfm = trfmLookup[secCol.closestCollEntity];

        RopePhysicsUtilities.CheckDistAndRespond(colComp.isTrigger, segRadius, colTrfm.Position, colComp.radius, ref firstTempTrfm, ref firstCol);

        colComp = collCompLookup[secCol.closestCollEntity];
        colTrfm = trfmLookup[secCol.closestCollEntity];

        RopePhysicsUtilities.CheckDistAndRespond(colComp.isTrigger, segRadius, colTrfm.Position, colComp.radius, ref secTempTrfm, ref secCol);
    }

    [BurstCompile]
    public void ApplyConstraintsAngular(in float segDist, in DynamicBuffer<RopeSegEntityBuffer> arrSegEntity, in float segDistSqr, in float segRadius)
    {
        TempLocalTransform firstTempTrfm, secTempTrfm;
        RopeSegCollisionComp firstCol, secCol;

        for (int i = 0; i < (arrSegEntity.Length - 1); i++)
        {
            firstTempTrfm = tempTrfmLookup[arrSegEntity[i].segEntity];
            secTempTrfm = tempTrfmLookup[arrSegEntity[i + 1].segEntity];

            firstCol = segCollCompLookup[arrSegEntity[i].segEntity];
            secCol = segCollCompLookup[arrSegEntity[i + 1].segEntity];

            if (firstCol.isCol && secCol.isCol)
                ApplyDeltaAngle(ref firstTempTrfm, ref secTempTrfm, trfmLookup[firstCol.closestCollEntity].Position, collCompLookup[firstCol.closestCollEntity].radius, segDistSqr, segRadius);
            else
                ApplyDeltaLinear(segDist, segRadius, ref firstTempTrfm, ref secTempTrfm, ref firstCol, ref secCol);

            tempTrfmLookup[arrSegEntity[i].segEntity] = firstTempTrfm;
            tempTrfmLookup[arrSegEntity[i + 1].segEntity] = secTempTrfm;

            segCollCompLookup[arrSegEntity[i].segEntity] = firstCol;
            segCollCompLookup[arrSegEntity[i + 1].segEntity] = secCol;
        }

        firstTempTrfm = tempTrfmLookup[arrSegEntity[^1].segEntity];
        secTempTrfm = tempTrfmLookup[arrSegEntity[0].segEntity];

        firstCol = segCollCompLookup[arrSegEntity[^1].segEntity];
        secCol = segCollCompLookup[arrSegEntity[0].segEntity];

        if (firstCol.isCol && secCol.isCol)
            ApplyDeltaAngle(ref firstTempTrfm, ref secTempTrfm, trfmLookup[firstCol.closestCollEntity].Position, collCompLookup[firstCol.closestCollEntity].radius, segDistSqr, segRadius);
        else
            ApplyDeltaLinear(segDist, segRadius, ref firstTempTrfm, ref secTempTrfm, ref firstCol, ref secCol);

        tempTrfmLookup[arrSegEntity[^1].segEntity] = firstTempTrfm;
        tempTrfmLookup[arrSegEntity[0].segEntity] = secTempTrfm;

        segCollCompLookup[arrSegEntity[^1].segEntity] = firstCol;
        segCollCompLookup[arrSegEntity[0].segEntity] = secCol;
    }

    [BurstCompile]
    public void ApplyDeltaAngle(ref TempLocalTransform firstTempTrfm, ref TempLocalTransform secTempTrfm, in float3 colPos, in float colRadius, in float segDistSqr, in float segRadius)
    {
        float angleReq = GetAngleRadians(segDistSqr, segRadius + colRadius);

        float curAngle = GetAngleRadians(math.distancesq(firstTempTrfm.Position, secTempTrfm.Position), segRadius + colRadius);

        float deltaAngle = curAngle - angleReq;

        float3 firstTempTrfmDir = firstTempTrfm.Position - colPos;

        float3 secTempTrfmDir = secTempTrfm.Position - colPos;

        float det = firstTempTrfmDir.x * secTempTrfmDir.y - firstTempTrfmDir.y * secTempTrfmDir.x;
        if (det > 0)
            det = 1;
        else
            det = -1;

        quaternion rotation = quaternion.AxisAngle(new float3(0, 0, det), deltaAngle / 2);

        float3 newfirstTempTrfmDir = math.rotate(rotation, firstTempTrfmDir);
        float3 newsecTempTrfmDir = math.rotate(math.inverse(rotation), secTempTrfmDir);

        newfirstTempTrfmDir += colPos;
        newsecTempTrfmDir += colPos;

        firstTempTrfm.Position.x = newfirstTempTrfmDir.x;
        firstTempTrfm.Position.y = newfirstTempTrfmDir.y;

        secTempTrfm.Position.x = newsecTempTrfmDir.x;
        secTempTrfm.Position.y = newsecTempTrfmDir.y;
    }

    [BurstCompile]
    public float GetAngleRadians(in float segDistSqr, in float equalSide)
    {
        float a_2 = 2 * equalSide * equalSide;

        // Avoid division by zero
        if (a_2 == 0)
            return 0;

        float cosTheta = (a_2 - segDistSqr) / a_2;

        // Clamp the value to prevent NaN
        cosTheta = math.clamp(cosTheta, -1f, 1f);

        float thetaRadians = math.acos(cosTheta);

        return thetaRadians;
    }
}