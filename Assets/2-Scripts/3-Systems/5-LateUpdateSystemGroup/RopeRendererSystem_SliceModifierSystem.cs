using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderFirst = true)]
[BurstCompile]
partial struct RopeRendererSystem_SliceModifierSystem : ISystem
{
    EntityQuery ropeQuery;
    RopeRendererSystem ropeRenSystem;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ropeQuery = SystemAPI.QueryBuilder().WithAll<
            RopeBasicComp,
            LineRendererCompanionLinkComp,
            RopeSegEntityBuffer, RopeSegPosBuffer>().WithAll<RopeSlicedComp>().Build();
        
        state.RequireForUpdate(ropeQuery);

        ropeRenSystem = new();
    }

    public void OnUpdate(ref SystemState state)
    {
        UpdatePosBuffer(ref state);

        foreach (var (_ropeBasicComp, ropeSegPosBuffer, _ropeLineRen, ropeEntity) in SystemAPI.Query<RefRO<RopeBasicComp>, DynamicBuffer<RopeSegPosBuffer>, RefRW<LineRendererCompanionLinkComp>>().WithAll<RopeSlicedComp>().WithEntityAccess())
        {
            ropeRenSystem.UpdateLineRenderer(_ropeBasicComp, _ropeLineRen, ropeSegPosBuffer, ropeEntity, ref state);
        }
    }

    [BurstCompile]
    public void UpdatePosBuffer(ref SystemState state)
    {
        NativeArray<Entity> _ropeEntities = ropeQuery.ToEntityArray(Allocator.TempJob);
        RopeRenderer_SliceModifierJob renJob = new()
        {
            ropeEntities = _ropeEntities,
            segEntityBufferLookup = SystemAPI.GetBufferLookup<RopeSegEntityBuffer>(true),
            tempTrfmCompLookup = SystemAPI.GetComponentLookup<TempLocalTransform>(true),
            ropeSlicedCompLookup = SystemAPI.GetComponentLookup<RopeSlicedComp>(true),
            lineRenCompLookup = SystemAPI.GetComponentLookup<LineRendererCompanionLinkComp>(),
            segPosBufferLookup = SystemAPI.GetBufferLookup<RopeSegPosBuffer>()
        };

        state.Dependency = renJob.ScheduleParallel(_ropeEntities.Length, 1, state.Dependency);
        state.CompleteDependency();

        _ropeEntities.Dispose();
    }
}
