using Unity.Mathematics;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct RopeApplyConstraints_SliceModifierJob : IJobFor
{
    [NativeDisableParallelForRestriction]
    public ComponentLookup<TempLocalTransform> tempTrfmLookup;

    [ReadOnly] public ComponentLookup<RopeBasicComp> ropeBasicCompLookup;
    [ReadOnly] public ComponentLookup<RopePhysComp> ropePhysCompLookup;
    [ReadOnly] public ComponentLookup<RopeSlicedComp> sliceCompLookup;

    [ReadOnly] public BufferLookup<RopeSegEntityBuffer> segEntityBufferLookup;

    [ReadOnly]
    public NativeArray<Entity> arrRopeEntities;

    [BurstCompile]
    public void Execute(int index)
    {
        Entity ropeEntity = arrRopeEntities[index];
        RopeBasicComp ropeBasicComp = ropeBasicCompLookup[ropeEntity];
        RopePhysComp ropePhysComp = ropePhysCompLookup[ropeEntity];
        RopeSlicedComp ropeSliceComp = sliceCompLookup[ropeEntity];

        DynamicBuffer<RopeSegEntityBuffer> arrSegEntity = segEntityBufferLookup[ropeEntity];

        if (!ropePhysComp.applyConst)
            return;

        if (arrSegEntity.Length <= 1)
            return;

        for (int k = 0; k < ropePhysComp.iterCount; k++)
        {
            ApplyConstraints(ropeSliceComp.sliceIndex, ropeBasicComp.segDist, ropeBasicComp.segRadius, arrSegEntity);
        }
    }

    [BurstCompile]
    public void ApplyConstraints(in int sliceIndex, in float segDist, in float segRadius, in DynamicBuffer<RopeSegEntityBuffer> arrSegEntity)
    {
        TempLocalTransform firstTempTrfm, secTempTrfm;

        int len = arrSegEntity.Length;
        for (int k = 0; k < len - 1; k++)
        {
            int i = (sliceIndex + k) % len;
            int j = (i + 1) % len;

            firstTempTrfm = tempTrfmLookup[arrSegEntity[i].segEntity];
            secTempTrfm = tempTrfmLookup[arrSegEntity[j].segEntity];

            ApplyDeltaLinear(segDist, segRadius, ref firstTempTrfm, ref secTempTrfm);

            tempTrfmLookup[arrSegEntity[i].segEntity] = firstTempTrfm;
            tempTrfmLookup[arrSegEntity[j].segEntity] = secTempTrfm;
        }
    }

    [BurstCompile]
    public void ApplyDeltaLinear(in float segDist, in float segRadius, ref TempLocalTransform firstTempTrfm, ref TempLocalTransform secTempTrfm)
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
    }
}