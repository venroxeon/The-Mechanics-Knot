using Unity.Burst;
using Unity.Entities;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(BeginFixedUpdateSystemGroup))]
[UpdateAfter(typeof(ResetSystem))]
[BurstCompile]
partial struct SetVelocityActiveRopeSystem : ISystem
{
    EntityQuery query;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        query = QueryBuilder().WithAll<SetVelocityActiveRopeComp, RopeSegEntityBuffer>().Build();
        state.RequireForUpdate(query);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SetVelocityActiveRopeJob setVelJob = new()
        {
            segVelCompLookup = GetComponentLookup<RopeSegVelocityComp>()
        };

        state.Dependency = setVelJob.ScheduleParallel(query, state.Dependency);
    }
}
