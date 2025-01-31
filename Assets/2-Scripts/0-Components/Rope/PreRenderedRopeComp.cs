using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using Unity.Mathematics;

[BurstCompile]
public struct PreRenderedRopeComp : IComponentData
{
    public int levelIndex, segCount;
    public float2 ropeCenter;
    public float semiMajor, semiMinor, segDist;
}
