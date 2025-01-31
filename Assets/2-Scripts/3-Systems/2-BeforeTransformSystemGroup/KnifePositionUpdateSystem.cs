using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(BeforeTransformSystemGroup))]
[BurstCompile]
partial struct KnifePositionUpdateSystem : ISystem
{
    bool isSecTouchActive;
    Entity knifeEntity;
    EntityArchetype archeType;

    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //query = QueryBuilder().WithAll<KnifeTag>().Build();
        
        //state.RequireForUpdate(query);
        state.RequireForUpdate<InputComp>();

        archeType = state.EntityManager.CreateArchetype(ComponentType.ReadWrite<KnifeComp>(), ComponentType.ReadWrite<TempLocalTransform>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if(TryGetSingleton<InputComp>(out var input))
        {
            if(isSecTouchActive)
            {
                if(input.isSecTouchPress)
                {
                    var tempTrfm = GetComponent<TempLocalTransform>(knifeEntity);
                    var knifeComp = GetComponent<KnifeComp>(knifeEntity);

                    knifeComp.prevPos = tempTrfm.Position;
                    tempTrfm.Position = input.secTouchPos;

                    GetComponentRW<TempLocalTransform>(knifeEntity).ValueRW = tempTrfm;
                    GetComponentRW<KnifeComp>(knifeEntity).ValueRW = knifeComp;
                }
                else
                {
                     GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).DestroyEntity(knifeEntity);

                    isSecTouchActive = false;
                }
            }
            else if(input.isSecTouchPress)
            {
                knifeEntity = state.EntityManager.CreateEntity(archeType);

                SetComponent<TempLocalTransform>(knifeEntity, new()
                {
                    Position = input.secTouchPos
                });

                SetComponent<KnifeComp>(knifeEntity, new()
                {
                    prevPos = input.secTouchPos
                });

                isSecTouchActive = true;
            }
        }
    }
}
