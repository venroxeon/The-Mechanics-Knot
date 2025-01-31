using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using static Unity.Entities.SystemAPI;
using UnityEngine;

[UpdateInGroup(typeof(AfterTransformSystemGroup))]
[UpdateBefore(typeof(RopeSliceDetectionSystem))]
[BurstCompile]
partial struct SpikeToNodeLinkSystem : ISystem
{
    EntityQuery query;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        query = QueryBuilder().WithAll<SpikeToNodeLinkComp, LocalTransform>().Build();
        state.RequireForUpdate(query);
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (_spikeDataComp, spriteRen, _Entity) in Query<RefRW<SpikeToNodeLinkComp>, ManagedAPI.UnityEngineComponent<SpriteRenderer>>().WithEntityAccess())
        {
            //GETTER
            var spikeDataComp = _spikeDataComp.ValueRO;
            ///

            GetComponentRW<LocalTransform>(_Entity).ValueRW.Rotation = GetComponent<LocalTransform>(spikeDataComp.linkedNode).Rotation;
            spikeDataComp.isActive = GetComponent<NodeRotatorComp>(spikeDataComp.linkedNode).isRotating;

            if(spikeDataComp.isActive)
            {
                var col = spriteRen.Value.color;
                col.a = 1;
                spriteRen.Value.color = col;
            }
            else
            {
                var col = spriteRen.Value.color;
                col.a = 0.5f;
                spriteRen.Value.color = col;
            }

            //SETTER
            _spikeDataComp.ValueRW = spikeDataComp;
        }
    }
}
