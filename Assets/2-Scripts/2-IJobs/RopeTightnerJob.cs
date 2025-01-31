using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;

[BurstCompile]
public struct RopeTightnerJob : IJobFor
{
    [NativeDisableParallelForRestriction]
    public ComponentLookup<RopeTightnerComp> ropeTightnerCompLookup;
    [NativeDisableParallelForRestriction]
    public BufferLookup<RopeSegEntityBuffer> segBufferLookup;

    [ReadOnly] public float deltaTime;

    [ReadOnly] public NativeArray<Entity> arrRopes;

    [ReadOnly] public ComponentLookup<TempLocalTransform> tempTrfmLookup;
    [ReadOnly] public ComponentLookup<RopeSegCollisionComp> segCollCompLookup;
    [ReadOnly] public ComponentLookup<RopeBasicComp> ropeBasicCompLookup;

    public EntityCommandBuffer.ParallelWriter ECB;

    [BurstCompile]
    public void Execute(int sortKey)
    {
        var ropeEntity = arrRopes[sortKey];
        var segBuffer = segBufferLookup[ropeEntity];
        var tightnerComp = ropeTightnerCompLookup[ropeEntity];
        var basicComp = ropeBasicCompLookup[ropeEntity];

        tightnerComp.curTotalTime += deltaTime;

        bool tighten = true;
        
        if (tightnerComp.curTotalTime < tightnerComp.reelDelayDuration)
            tighten = false;
        else
            tightnerComp.curTotalTime -= tightnerComp.reelDelayDuration;

        ropeTightnerCompLookup[ropeEntity] = tightnerComp;
        
        if (segBuffer.Length <= 3)
            tighten = false;

        if (tighten)
        {
            float segDistSqrMin = basicComp.segDistSqr;

            float totalSegDistSqr = 0, count = 0;
            for (int k = 0; k < segBuffer.Length - 1; k++)
            {
                int i = k % segBuffer.Length;
                int j = (i + 1) % segBuffer.Length;

                var firstSeg = segBuffer[i].segEntity;
                var secSeg = segBuffer[j].segEntity;

                if (segCollCompLookup[firstSeg].isCol || segCollCompLookup[secSeg].isCol)
                    continue;

                var firstTempTrfmComp = tempTrfmLookup[firstSeg];
                var secTempTrfmComp = tempTrfmLookup[secSeg];

                float sqrDist = math.distancesq(firstTempTrfmComp.Position, secTempTrfmComp.Position);

                totalSegDistSqr += sqrDist;
                count++;
            }

            totalSegDistSqr /= count;

            if(totalSegDistSqr < segDistSqrMin + tightnerComp.extraLenMin)
            {
                int incr = (segBuffer.Length / tightnerComp.removeCount) + 1;
                for (int j = 0; j < segBuffer.Length; j += incr)
                {
                    var segEntity = segBuffer[j].segEntity;
                    segBuffer.RemoveAt(j);
                    ECB.DestroyEntity(sortKey, segEntity);
                }
            }
            else if(totalSegDistSqr > segDistSqrMin + (tightnerComp.extraLenMax))
            {
                //AddSegment(sortKey, ropeEntity, basicComp, tempTrfmLookup[segBuffer[^1].segEntity].Position);
            }
        }

    }

    [BurstCompile]
    public RopeSegEntityBuffer AddSegment(int sortKey, in Entity ropeEntity, in RopeBasicComp ropeBasicComp, in float3 newPos)
    {
        var segEntity = ECB.Instantiate(sortKey, ropeBasicComp.segPref);

        var tempTrfm = tempTrfmLookup[ropeBasicComp.segPref];
        tempTrfm.Position = newPos;

        ECB.SetComponent(sortKey, segEntity, tempTrfm);
        ECB.SetComponent<RopeSegVelocityComp>(sortKey, segEntity, new()
        {
            prevPos = newPos
        });

        ECB.SetComponent<RopeSegParEntityComp>(sortKey, segEntity, new() { parEntity = ropeEntity });
        ECB.AddComponent<RopeGenerationCompleteTag>(sortKey, segEntity);

        //UPDATE PARENT ROPE BUFFERS
        ECB.AppendToBuffer<RopeSegEntityBuffer>(sortKey, ropeEntity, new()
        {
            segEntity = segEntity
        });
        ECB.AppendToBuffer<LinkedEntityGroup>(sortKey, ropeEntity, new()
        {
            Value = segEntity
        });
        ///

        ECB.RemoveComponent<LocalTransform>(sortKey, segEntity);

        return new RopeSegEntityBuffer() { segEntity = segEntity };
    }
}
