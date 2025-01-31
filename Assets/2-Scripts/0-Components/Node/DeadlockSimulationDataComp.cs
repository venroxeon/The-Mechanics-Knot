using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct DeadlockSimulationDataComp : IComponentData
{
    public bool isAssigned;
    public int assignedDir;
}
