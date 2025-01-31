using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct InterpolationWithLocalTransformJob : IJobEntity
{
    public float deltaTime, fixedDeltaTime;

    [BurstCompile]
    public void Execute(ref InterpolationComp interComp, ref LocalTransform trfm)
    {
        if (interComp.SetTargetValuesIfChanged(trfm))
        {
            interComp.accumulator += deltaTime;
            interComp.accumulator = math.min(interComp.accumulator, fixedDeltaTime);

            var interpolationFactor = interComp.accumulator / fixedDeltaTime;

            if (interComp.forPosition)
            {
                trfm.Position = math.lerp(interComp.prevPosition, interComp.targetPosition, interpolationFactor);
            }

            if (interComp.forRotation)
            {
                trfm.Rotation = UnityEngine.Quaternion.LerpUnclamped(interComp.prevRotation, interComp.targetRotation, interpolationFactor);
            }
        }
    }
}

[BurstCompile]
public partial struct InterpolationWithoutLocalTransformJob : IJobEntity
{
    public float fixedDeltaTime, deltaTime;

    [BurstCompile]
    public void Execute(ref TempLocalTransform tempTrfm, ref InterpolationComp interComp)
    {
        if(interComp.SetTargetValuesIfChanged(tempTrfm))
        {
            interComp.accumulator += deltaTime;
            interComp.accumulator = math.min(interComp.accumulator, fixedDeltaTime);

            var interpolationFactor = interComp.accumulator / fixedDeltaTime;

            if (interComp.forPosition)
            {
                tempTrfm.Position = math.lerp(interComp.prevPosition, interComp.targetPosition, interpolationFactor);
            }

            if (interComp.forRotation)
            {
                tempTrfm.Rotation = UnityEngine.Quaternion.Lerp(interComp.prevRotation, interComp.targetRotation, interpolationFactor);
            }
        }
    }
}