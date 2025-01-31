using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderFirst = true)]
[BurstCompile]
partial struct RopeRendererSystem : ISystem
{
    EntityQuery ropeQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ropeQuery = SystemAPI.QueryBuilder().WithAll<
            RopeBasicComp, 
            LineRendererCompanionLinkComp, 
            RopeSegEntityBuffer, RopeSegPosBuffer>().WithNone<RopeSlicedComp>().Build();
        state.RequireForUpdate(ropeQuery);
    }

    public void OnUpdate(ref SystemState state)
    {
        UpdatePosBuffer(ref state);

        foreach (var (_ropeBasicComp, ropeSegPosBuffer, _ropeLineRen, ropeEntity) in SystemAPI.Query<RefRO<RopeBasicComp>, DynamicBuffer<RopeSegPosBuffer>, RefRW<LineRendererCompanionLinkComp>>().WithNone<RopeSlicedComp>().WithEntityAccess())
        {
            UpdateLineRenderer(_ropeBasicComp, _ropeLineRen, ropeSegPosBuffer, ropeEntity, ref state);
        }
    }

    [BurstCompile]
    public void UpdatePosBuffer(ref SystemState state)
    {
        NativeArray<Entity> _ropeEntities = ropeQuery.ToEntityArray(Allocator.TempJob);
        RopeRendererJob renJob = new()
        {
            ropeEntities = _ropeEntities,
            segEntityBufferLookup = SystemAPI.GetBufferLookup<RopeSegEntityBuffer>(true),
            tempTrfmCompLookup = SystemAPI.GetComponentLookup<TempLocalTransform>(true),
            segPosBufferLookup = SystemAPI.GetBufferLookup<RopeSegPosBuffer>(),
            lineRenCompLookup = SystemAPI.GetComponentLookup<LineRendererCompanionLinkComp>()
        };

        state.Dependency = renJob.ScheduleParallel(_ropeEntities.Length, 1, state.Dependency);
        state.CompleteDependency();

        _ropeEntities.Dispose();
    }

    public void UpdateLineRenderer(in RefRO<RopeBasicComp> _ropeBasicComp, in RefRW<LineRendererCompanionLinkComp> _ropeLineRen, in DynamicBuffer<RopeSegPosBuffer> ropeSegPosBuffer, Entity ropeEntity, ref SystemState state)
    {
        //GETTER
        RopeBasicComp ropeBasicComp = _ropeBasicComp.ValueRO;
        LineRendererCompanionLinkComp ropeLineRen = _ropeLineRen.ValueRO;
        //GETTER

        LineRenderer lineRen;
        if (!ropeLineRen.isPrefSpawned)
        {
            var obj = Object.Instantiate(ropeLineRen.lineRenPref.Value);

            lineRen = obj.GetComponent<LineRenderer>();
            lineRen.startWidth = ropeBasicComp.segRadius * 2;
            lineRen.endWidth = ropeBasicComp.segRadius * 2;

            ropeLineRen.isPrefSpawned = true;
            ropeLineRen.lineRen = lineRen;
        }
        else
        {
            lineRen = ropeLineRen.lineRen;

            if (!ropeLineRen.isLoopSet)
            {
                if (!SystemAPI.HasComponent<RopeGeneratorComp>(ropeEntity))
                {
                    ropeLineRen.isLoopSet = true;
                    //lineRen.loop = true;
                }
            }
        }

        lineRen.positionCount = ropeLineRen.count;
        lineRen.SetPositions(ropeSegPosBuffer.Reinterpret<Vector3>().AsNativeArray());

        //SETTER
        _ropeLineRen.ValueRW = ropeLineRen;
    }
}
