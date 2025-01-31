using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public partial struct InterpolationDataUpdationWithTransformJob : IJobEntity
{
    [BurstCompile]
    public void Execute(ref LocalTransform trfm, ref InterpolationComp interComp)
    {
        interComp.SetPreviousValues(ref trfm);
    }
}

[BurstCompile]
public partial struct InterpolationDataUpdationWithoutTransformJob : IJobEntity
{
    [BurstCompile]
    public void Execute(ref TempLocalTransform tempTrfm, ref InterpolationComp interComp)
    {
        interComp.SetPreviousValues(ref tempTrfm);
    }
}
