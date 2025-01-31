using Unity.Entities;
using Unity.Burst;

[UpdateInGroup(typeof(RopePhysicsSimulationSystemGroup))]
[UpdateBefore(typeof(RopeApplyConstraintsSystem))]
[BurstCompile]
partial struct RopeSegVelocitySystem : ISystem
{
    float timestep;
    EntityQuery velQuery;

    public void OnCreate(ref SystemState state)
    {
        velQuery = SystemAPI.QueryBuilder().WithAll<
            RopeGenerationCompleteTag, 
            TempLocalTransform, RopeSegVelocityComp>().Build();

        state.RequireForUpdate(velQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.TryGetSingleton<FixedStepSimulationParamComp>(out var fixedTimeComp))
        {
            RopeSegVelocityJob velJob = new()
            {
                fixedDeltaTime = fixedTimeComp.fixedDeltaTime,
                ropePhysCompLookup = SystemAPI.GetComponentLookup<RopePhysComp>(true), 
                ropeSegParEntityLookup = SystemAPI.GetComponentLookup<RopeSegParEntityComp>(true)
            };

            state.Dependency = velJob.ScheduleParallel(velQuery, state.Dependency);
        }
    }
}