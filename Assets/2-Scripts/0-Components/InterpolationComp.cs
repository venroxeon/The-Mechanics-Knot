using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct InterpolationComp : IComponentData
{
    public bool hasChanged, isInitialized;

    public bool forPosition, forRotation, forScale;

    public float prevScale, targetScale, accumulator;

    public float3 prevPosition, targetPosition;

    public quaternion prevRotation, targetRotation;
}

public static class InterpolationCompExtensions
{
    public static bool SetTargetValuesIfChanged(ref this InterpolationComp interComp, in LocalTransform trfm)
    {
        if (!interComp.hasChanged) return interComp.isInitialized;

        interComp.targetPosition = trfm.Position;
        interComp.targetRotation = trfm.Rotation;
        interComp.targetScale = trfm.Scale;

        interComp.hasChanged = false;

        return interComp.isInitialized;
    }

    public static bool SetTargetValuesIfChanged(ref this InterpolationComp interComp, in TempLocalTransform tempTrfm)
    {
        if (!interComp.hasChanged)
            return interComp.isInitialized;

        interComp.targetPosition = tempTrfm.Position;
        interComp.targetRotation = tempTrfm.Rotation;
        interComp.targetScale = tempTrfm.Scale;

        interComp.hasChanged = false;

        return interComp.isInitialized;
    }

    public static void SetPreviousValues(ref this InterpolationComp interComp, ref LocalTransform trfm)
    {
        interComp.accumulator = 0;

        //UPDATE PREVIOUS VALUES ONLY FOR THE FIRST FIXED UPDATE PER FRAME
        if (!interComp.hasChanged)
        {
            interComp.prevPosition = trfm.Position;
            interComp.prevRotation = trfm.Rotation;
            interComp.prevScale = trfm.Scale;

            //SET CURRENT TRANSFORM TO THE TARGET TRANSFORM FROM LAST FRAME, IN CASE IF INTERPOLATION FACTOR DOES NOT REACH 1
            if (interComp.isInitialized)
            {
                trfm.Position = interComp.targetPosition;
                trfm.Rotation = interComp.targetRotation;
                trfm.Scale = interComp.targetScale;
            }

            interComp.hasChanged = true;
            interComp.isInitialized = true;
        }
    }

    public static void SetPreviousValues(ref this InterpolationComp interComp, ref TempLocalTransform tempTrfm)
    {
        interComp.accumulator = 0;

        //UPDATE PREVIOUS VALUES ONLY FOR THE FIRST FIXED UPDATE PER FRAME
        if (!interComp.hasChanged)
        {
            interComp.prevPosition = tempTrfm.Position;
            interComp.prevRotation = tempTrfm.Rotation;
            interComp.prevScale = tempTrfm.Scale;

            //SET CURRENT TRANSFORM TO THE TARGET TRANSFORM FROM LAST FRAME, IN CASE IF INTERPOLATION FACTOR DOES NOT REACH 1
            
            if (interComp.isInitialized)
            {
                tempTrfm.Position = interComp.targetPosition;
                tempTrfm.Rotation = interComp.targetRotation;
                tempTrfm.Scale = interComp.targetScale;
            }

            interComp.hasChanged = true;
            interComp.isInitialized = true;
        }

    }
}