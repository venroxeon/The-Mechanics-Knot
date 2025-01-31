using Unity.Mathematics;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;

[BurstCompile]
public partial struct RopeSegVelocityJob : IJobEntity
{
    public float fixedDeltaTime;
    [ReadOnly] public ComponentLookup<RopePhysComp> ropePhysCompLookup;
    [ReadOnly] public ComponentLookup<RopeSegParEntityComp> ropeSegParEntityLookup;

    [BurstCompile]
    public void Execute(in Entity segEntity, ref TempLocalTransform tempTrfm, ref RopeSegVelocityComp velComp)
    {
        float3 vel = tempTrfm.Position - velComp.prevPos;
        vel += ropePhysCompLookup[ropeSegParEntityLookup[segEntity].parEntity].gravity * fixedDeltaTime;

        velComp.prevPos = tempTrfm.Position;
        if(velComp.hasVelocity)
            tempTrfm.Position += vel;
    }
}