using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct NodeEvaluationDataComp : IComponentData
{
    public int targetRotDir;
}
