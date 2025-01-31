using Unity.Entities;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
public struct LineRendererCompanionLinkComp : IComponentData
{
    public bool isPrefSpawned, isLoopSet;
    public int count;
    public UnityObjectRef<LineRenderer> lineRen;
    public UnityObjectRef<GameObject> lineRenPref;
}