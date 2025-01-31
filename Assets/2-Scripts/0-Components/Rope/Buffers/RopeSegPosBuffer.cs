using Unity.Entities;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public struct RopeSegPosBuffer : IBufferElementData
{
    public Vector3 segPos;
}
