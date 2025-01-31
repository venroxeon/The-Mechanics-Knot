using Unity.Entities;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public partial struct SetVelocityActiveRopeJob : IJobEntity
{
    [NativeDisableParallelForRestriction]
    public ComponentLookup<RopeSegVelocityComp> segVelCompLookup;

    [BurstCompile]
    public void Execute(in SetVelocityActiveRopeComp setVelComp, in DynamicBuffer<RopeSegEntityBuffer> segBuffer)
    {
        foreach (var segElem in segBuffer)
            segVelCompLookup.GetRefRW(segElem.segEntity).ValueRW.hasVelocity = setVelComp.isActive;
    }
}
