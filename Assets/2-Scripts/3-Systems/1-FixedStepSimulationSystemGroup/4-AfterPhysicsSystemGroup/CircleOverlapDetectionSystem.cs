using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[BurstCompile]
partial struct CircleOverlapDetectionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CircleCollisionDetectionComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ECB = new(Unity.Collections.Allocator.Temp);

        foreach(var (circleCollComp, _Trfm, _Entity) in Query<RefRO<CircleCollisionDetectionComp>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            var physWorld = GetSingleton<PhysicsWorldSingleton>();

            var startPos = _Trfm.ValueRO.Position;

            startPos.z -= 1;

            var colFilter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u
            };

            var circleRadius = circleCollComp.ValueRO.radius;

            if (physWorld.CollisionWorld.SphereCast(startPos, circleRadius, new(0, 0, 1), 2, out var hit, colFilter))
            {
                if (hit.Entity != _Entity)
                {
                    if(!HasComponent<CollidedTag>(_Entity))
                    {
                        ECB.AddComponent<CollidedTag>(_Entity);
                    }
                }
                else if(HasComponent<CollidedTag>(_Entity))
                {
                    ECB.RemoveComponent<CollidedTag>(_Entity);
                }
            }
            else if(HasComponent<CollidedTag>(_Entity))
            {
                ECB.RemoveComponent<CollidedTag>(_Entity);
            }
        }

        if (!ECB.IsEmpty)
            ECB.Playback(state.EntityManager);

        ECB.Dispose();
    }
}
