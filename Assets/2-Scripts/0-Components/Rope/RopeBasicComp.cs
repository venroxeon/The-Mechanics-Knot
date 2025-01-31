using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct RopeBasicComp : IComponentData 
{
    public int segLimit;
    public float segDist, segDistSqr, segRadius;
    public Entity segPref;
}