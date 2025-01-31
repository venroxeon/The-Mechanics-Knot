using Unity.Entities;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public struct LevelBuffer : IBufferElementData
{
    public Entity levelEntity;
}
