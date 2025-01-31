using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(AfterTransformSystemGroup))]
[BurstCompile]
partial struct DestroyProxyNodeSystem : ISystem
{
    EntityQuery query;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
	    state.RequireForUpdate<InputComp>();
	    state.RequireForUpdate<LevelDataForProxyNodeComp>();
	    state.RequireForUpdate<LevelManagerDataComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        InputComp input = GetSingleton<InputComp>();

        if (!input.isSecTouchPress)
            return;

        var rayFrom = input.secTouchPos;
        rayFrom.z = -2;
        var rayTo = input.secTouchPos;
        rayTo.z += 2;

        if(CustomUtilities.Utilities.Raycast(rayFrom, rayTo, out var hitEntity, GetSingleton<PhysicsWorldSingleton>().CollisionWorld))
        {
            if (HasComponent<ProxyNodeTag>(hitEntity))
            {
                GetComponentRW<LevelDataForProxyNodeComp>(GetSingleton<LevelManagerDataComp>().curLevelEntity).ValueRW.curProxyNodeCount--;
                state.EntityManager.DestroyEntity(hitEntity);
            }
        }
    }
}
