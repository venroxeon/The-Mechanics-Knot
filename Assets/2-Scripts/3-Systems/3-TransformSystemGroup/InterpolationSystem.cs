using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(TransformSystemGroup), OrderFirst = true)]
[BurstCompile]
partial struct InterpolationSystem : ISystem
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
        if(TryGetSingleton<FixedStepSimulationParamComp>(out var fixedStepComp))
        {
            InterpolationWithLocalTransformJob interJob1 = new()
            {
                fixedDeltaTime = fixedStepComp.fixedDeltaTime,
                deltaTime = Time.DeltaTime
            };
            InterpolationWithoutLocalTransformJob interJob2 = new()
            {
                fixedDeltaTime = fixedStepComp.fixedDeltaTime,
                deltaTime = Time.DeltaTime
            };

            state.Dependency = interJob1.ScheduleParallel(query1, state.Dependency);
            state.Dependency = interJob2.ScheduleParallel(query2, state.Dependency);
        }
    }
}
