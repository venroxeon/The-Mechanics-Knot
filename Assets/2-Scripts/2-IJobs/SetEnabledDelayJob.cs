using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using CustomUtilities;

[BurstCompile]
public partial struct SetEnabledDelayJob : IJobEntity
{
    [ReadOnly] public float DeltaTime;
    public EntityCommandBuffer ECB;

    [ReadOnly] public EntityStorageInfoLookup storageLookup;

    [BurstCompile]
    public void Execute(in Entity _Entity, ref SetEnabledDelayComp delayComp, ref DynamicBuffer<LinkedEntityGroup> linkedGroup)
    {
        delayComp.curTotalTime += DeltaTime;

        if (delayComp.curTotalTime >= delayComp.delayTime)
        {
            Utilities.UpdateLinkedEntityGroup(ref linkedGroup, storageLookup);

            ECB.SetEnabled(_Entity, delayComp.isEnabled);
            ECB.RemoveComponent<SetEnabledDelayComp>(_Entity);
        }
    }
}
