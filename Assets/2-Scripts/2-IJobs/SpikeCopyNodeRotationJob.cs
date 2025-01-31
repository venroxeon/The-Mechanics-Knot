using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;

[BurstCompile]
public partial struct SpikeToNodeLinkJob : IJobEntity
{
    public ComponentLookup<LocalTransform> trfmLookup;
    [ReadOnly] public ComponentLookup<NodeRotatorComp> nodeRotCompLookup;

    [BurstCompile]
    public void Execute(in Entity _Entity, ref SpikeToNodeLinkComp spikeDataComp)
    {
        trfmLookup.GetRefRW(_Entity).ValueRW.Rotation = trfmLookup[spikeDataComp.linkedNode].Rotation;

        spikeDataComp.isActive = nodeRotCompLookup[spikeDataComp.linkedNode].isRotating;
    }
}
