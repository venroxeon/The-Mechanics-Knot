using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public struct ProxyNodePrefEntityComp : IComponentData
{
    public Entity proxyNodePref;
}
