using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using System;
using Unity.Mathematics;

[BurstCompile]
public partial struct RopeRenderer_SliceModifierJob : IJobFor
{
    [ReadOnly] public NativeArray<Entity> ropeEntities;

    [ReadOnly] public ComponentLookup<TempLocalTransform> tempTrfmCompLookup;
    [ReadOnly] public ComponentLookup<RopeSlicedComp> ropeSlicedCompLookup;
    [ReadOnly] public BufferLookup<RopeSegEntityBuffer> segEntityBufferLookup;
    
    [NativeDisableParallelForRestriction]
    public ComponentLookup<LineRendererCompanionLinkComp> lineRenCompLookup;
    [NativeDisableParallelForRestriction]
    public BufferLookup<RopeSegPosBuffer> segPosBufferLookup;

    [BurstCompile]
    public void Execute(int index)
    {
        //GETTER
        Entity ropeEntity = ropeEntities[index];
        RopeSlicedComp slicedComp = ropeSlicedCompLookup[ropeEntity];
        LineRendererCompanionLinkComp ropeLineRen = lineRenCompLookup[ropeEntity];

        DynamicBuffer<RopeSegEntityBuffer> ropeSegBuffer = segEntityBufferLookup[ropeEntity];
        DynamicBuffer<RopeSegPosBuffer> segPosBuffer = segPosBufferLookup[ropeEntity];
        //GETTER

        RopeSegPosBuffer bufferComp;
        int len = ropeSegBuffer.Length;

        for (int k = 0; k < len; k++)
        {
            int i = (slicedComp.sliceIndex + k) % len;
            float3 segPos = tempTrfmCompLookup[ropeSegBuffer[i].segEntity].Position;

            bufferComp.segPos = new float3(segPos.x, segPos.y, 0);
            segPosBuffer[k] = bufferComp;
        }

        ropeLineRen.count = len;

        lineRenCompLookup[ropeEntity] = ropeLineRen;
    }
}
