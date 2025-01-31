using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(BeginFixedUpdateSystemGroup))]
[BurstCompile]
partial struct InterpolationDataUpdationSystem : ISystem
{
    EntityQuery query1, query2;

    // [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        query1 = QueryBuilder().WithAll<LocalTransform, InterpolationComp>().Build();
        
        query2 = QueryBuilder().WithAll<TempLocalTransform, InterpolationComp>().Build();

        state.RequireAnyForUpdate(query1, query2);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        InterpolationDataUpdationWithTransformJob interDataJob1 = new();
        InterpolationDataUpdationWithoutTransformJob interDataJob2 = new();

        state.Dependency = interDataJob1.ScheduleParallel(query1, state.Dependency);
        state.Dependency = interDataJob2.ScheduleParallel(query2, state.Dependency);
    }
}
