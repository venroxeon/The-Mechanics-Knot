using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public partial struct RopeSliceDetectionJob : IJobFor
{
    public EntityCommandBuffer.ParallelWriter ECB;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<RopeSegVelocityComp> segVelCompLookup;
    [NativeDisableParallelForRestriction]
    public ComponentLookup<RopePhysComp> ropePhysCompLookup;

    [ReadOnly] public bool isKnifeActive;
    [ReadOnly] public float2 knifePos;

    [ReadOnly] public ComponentLookup<RopeBasicComp> ropeBasicCompLookup;
    [ReadOnly] public ComponentLookup<RopeSegCollisionComp> ropeSegCollisionCompLookup;
    [ReadOnly] public ComponentLookup<SpikeToNodeLinkComp> spikeTagLookup;
    [ReadOnly] public ComponentLookup<RopeSegParEntityComp> segParCompLookup;
    [ReadOnly] public ComponentLookup<TempLocalTransform> tempTrfmCompLookup;

    [ReadOnly] public BufferLookup<RopeSegEntityBuffer> segEntityBufferLookup;

    [ReadOnly] public NativeArray<Entity> arrRopeEntities;

    ComponentTypeSet removeTypeSet;

    [BurstCompile]
    public void Execute(int index)
    {
        removeTypeSet = new(ComponentType.ReadWrite<RopeSegCollisionComp>(), ComponentType.ReadWrite<RopeSegMovementComp>());

        Entity ropeEntity = arrRopeEntities[index];

        var arrSegEntity = segEntityBufferLookup[ropeEntity];

        var prevSegEntity = arrSegEntity[^1].segEntity;
        var prevSegTrfm = tempTrfmCompLookup[prevSegEntity];
        for (int i = 0; i < arrSegEntity.Length; i++)
        {
            var segEntity = arrSegEntity[i].segEntity;
            var segTrfm = tempTrfmCompLookup[segEntity];

            if(ropeSegCollisionCompLookup.TryGetComponent(segEntity, out var segCollComp))
            {
                bool hasHitKnife = false;
                if (isKnifeActive)
                    hasHitKnife = CustomUtilities.RopePhysicsUtilities.CheckDist(ropeBasicCompLookup[ropeEntity].segRadius, 0.1f, knifePos, new(segTrfm.Position.x, segTrfm.Position.y), out float dist, out float2 delta);

                bool hasHitSpike = false;
                if(segCollComp.hasEntered)
                {
                    if(spikeTagLookup.TryGetComponent(segCollComp.closestCollEntity, out var spikeData))
                    {
                        hasHitSpike = spikeData.isActive;
                    }
                }

                if (hasHitKnife || hasHitSpike)
                {
                    ECB.AddComponent<RopeSlicedComp>(index, segParCompLookup[segEntity].parEntity, new()
                    {
                        sliceIndex = i
                    });

                    //REMOVING UNWANTED COMPONENTS FROM PARENT ROPE
                    ECB.RemoveComponent<RopeNodeEntityBuffer>(index, segParCompLookup[segEntity].parEntity);
                    //

                    //REMOVING UNWANTED COMPONENTS FROM SEGMENTS
                    var arrSeg = arrSegEntity.Reinterpret<Entity>().AsNativeArray();
                    ECB.RemoveComponent(index, arrSeg, removeTypeSet);
                    ///

                    //APPLYING VELOCITY
                    ECB.AddComponent<SetVelocityActiveRopeComp>(index, ropeEntity, new() { isActive = true });
                    ///

                    //APPLYING SNAP EFFECT
                    ApplySnapEffect(ropeEntity, prevSegEntity, segEntity, prevSegTrfm, segTrfm);
                    ///

                    break;
                }
            }

            prevSegTrfm = segTrfm;
            prevSegEntity = segEntity;
        }
    }

    [BurstCompile]
    void ApplySnapEffect(in Entity ropeEntity, in Entity prevSegEntity, in Entity segEntity, in TempLocalTransform prevSegTrfm, in TempLocalTransform segTrfm)
    {
        var delta = prevSegTrfm.Position - segTrfm.Position;

        var segVel = segVelCompLookup[segEntity];
        var prevSegVel = segVelCompLookup[prevSegEntity];

        var physComp = ropePhysCompLookup[ropeEntity];

        segVel.prevPos = prevSegTrfm.Position + delta;
        prevSegVel.prevPos = segTrfm.Position - delta;

        physComp.gravity.y = -0.2f;

        segVelCompLookup[segEntity] = segVel;
        segVelCompLookup[prevSegEntity] = prevSegVel;

        ropePhysCompLookup[ropeEntity] = physComp;
    }
}
