using Unity.Entities;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public struct GameObjectRefComp : IComponentData
{
    public UnityObjectRef<GameObject> objRef;
}
