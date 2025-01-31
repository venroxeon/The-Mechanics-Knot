using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct RopeSpawnerComp : IComponentData
{
    public Entity ropePref;
}