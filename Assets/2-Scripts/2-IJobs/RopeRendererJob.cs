using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct RopeRendererJob : IJobFor
{
    [ReadOnly] public NativeArray<Entity> ropeEntities;

    [ReadOnly] public ComponentLookup<TempLocalTransform> tempTrfmCompLookup;
    [ReadOnly] public BufferLookup<RopeSegEntityBuffer> segEntityBufferLookup;

    [NativeDisableParallelForRestriction]
    public BufferLookup<RopeSegPosBuffer> segPosBufferLookup;
    [NativeDisableParallelForRestriction]
    public ComponentLookup<LineRendererCompanionLinkComp> lineRenCompLookup;

    [BurstCompile]
    public void Execute(int index)
    {
        //GETTER
        Entity ropeEntity = ropeEntities[index];
        LineRendererCompanionLinkComp ropeLineRen = lineRenCompLookup[ropeEntity];

        DynamicBuffer<RopeSegEntityBuffer> ropeSegBuffer = segEntityBufferLookup[ropeEntity];
        DynamicBuffer<RopeSegPosBuffer> segPosBuffer = segPosBufferLookup[ropeEntity];
        //GETTER

        int i;
        RopeSegPosBuffer bufferComp;
        for (i = 0; i < ropeSegBuffer.Length; i++)
        {
            float3 segPos = tempTrfmCompLookup[ropeSegBuffer[i].segEntity].Position;
            
            bufferComp.segPos = new float3(segPos.x, segPos.y, 0);
            segPosBuffer[i] = bufferComp;
        }

        int count;
        if (ropeLineRen.isLoopSet && ropeSegBuffer.Length > 2)
        {
            float3 segPos = tempTrfmCompLookup[ropeSegBuffer[0].segEntity].Position;
            bufferComp.segPos = new float3(segPos.x, segPos.y, 0);
            segPosBuffer[i] = bufferComp;

            segPos = tempTrfmCompLookup[ropeSegBuffer[1].segEntity].Position;
            bufferComp.segPos = new float3(segPos.x, segPos.y, 0);
            segPosBuffer[i + 1] = bufferComp;

            count = ropeSegBuffer.Length + 2;
        }
        else
            count = ropeSegBuffer.Length;

        ropeLineRen.count = count;

        lineRenCompLookup[ropeEntity] = ropeLineRen;
    }
}
