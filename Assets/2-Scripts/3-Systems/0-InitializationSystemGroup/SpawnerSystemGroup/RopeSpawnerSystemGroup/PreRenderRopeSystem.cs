using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Entities.SystemAPI;

[BurstCompile]
partial struct PreRenderRopeSystem : ISystem
{
    EntityQuery query;
    ComponentTypeSet trfmTypeSet;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
	    query = QueryBuilder().WithAll<PreRenderedRopeComp>().Build();
	    state.RequireForUpdate(query);

        trfmTypeSet = new(ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadWrite<LocalToWorld>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ECB = new EntityCommandBuffer(Allocator.Temp);

        foreach(var preRenRope in Query<RefRO<PreRenderedRopeComp>>())
        {
            var ropeEntity = Spawn(ref ECB, ref state);

            GenerateRope(ref ECB, preRenRope.ValueRO, ropeEntity, ref state);
        }

        state.EntityManager.RemoveComponent<PreRenderedRopeComp>(query);

        if (!ECB.IsEmpty)
            ECB.Playback(state.EntityManager);

        ECB.Dispose();
    }

    [BurstCompile]
    Entity Spawn(ref EntityCommandBuffer ECB, ref SystemState state)
    {
        var levelManager = GetSingleton<LevelManagerDataComp>();
        var curLevelData = GetComponent<LevelDataComp>(levelManager.curLevelEntity);

        var linkedBuffer = GetBuffer<LinkedEntityGroup>(levelManager.curLevelEntity);

        Entity ropeEntity = default;
        foreach (var (ropeSpawnerComp, _Entity) in SystemAPI.Query<RefRO<RopeSpawnerComp>>().WithEntityAccess())
        {
            ropeEntity = state.EntityManager.Instantiate(ropeSpawnerComp.ValueRO.ropePref);
            ECB.RemoveComponent<RopeGeneratorComp>(ropeEntity);

            ECB.AddComponent<Parent>(ropeEntity, new()
            {
                Value = _Entity
            });

            linkedBuffer.Add(ropeEntity);
        }

        return ropeEntity;
    }

    [BurstCompile]
    void GenerateRope(ref EntityCommandBuffer ECB, in PreRenderedRopeComp preRopeComp, in Entity ropeEntity, ref SystemState state)
    {
        Entity segPref = GetComponent<RopeBasicComp>(ropeEntity).segPref;

        NativeList<float3> points = new(Allocator.Temp);
        GenerateEllipsePoints(preRopeComp.semiMajor, preRopeComp.semiMinor, preRopeComp.segDist, preRopeComp.segCount, preRopeComp.ropeCenter, ref points);

        NativeArray<Entity> arrSeg = new(points.Length, Allocator.Temp);
        state.EntityManager.Instantiate(segPref, arrSeg);

        var segBuffer = GetBuffer<RopeSegEntityBuffer>(ropeEntity);
        var linkedEntityBuffer = GetBuffer<LinkedEntityGroup>(ropeEntity);

        int i = 0;

        foreach (Entity seg in arrSeg)
        {
            var tempTrfm = GetComponentRW<TempLocalTransform>(seg);

            float3 newPos = points[i++];
            tempTrfm.ValueRW.Position = newPos;

            GetComponentRW<RopeSegVelocityComp>(seg).ValueRW.prevPos = newPos;

            SetComponent(seg, new RopeSegParEntityComp() { parEntity = ropeEntity });

            ECB.RemoveComponent(seg, trfmTypeSet);

            segBuffer.Add(new RopeSegEntityBuffer() { segEntity = seg });

            linkedEntityBuffer.Add(new()
            {
                Value = seg
            });
        }

        NativeList<Entity> listRopeSeg = new(Allocator.Temp);

        listRopeSeg.AddRange(arrSeg);
        listRopeSeg.Add(ropeEntity);

        ECB.AddComponent<RopeGenerationCompleteTag>(listRopeSeg.AsArray());

        //UPDATE ROPE COUNT
        var levelManager = GetSingleton<LevelManagerDataComp>();
        var curLevelData = GetComponent<LevelDataComp>(levelManager.curLevelEntity);

        GetComponentRW<LevelDataComp>(levelManager.curLevelEntity).ValueRW.curRopeCount++;

        arrSeg.Dispose();
        listRopeSeg.Dispose();
        points.Dispose();
    }

    public void GenerateEllipsePoints(float a, float b, float gap, int maxPoints, in float2 ropeCenter, ref NativeList<float3> points)
    {
        // Calculate the circumference of the ellipse (approximation)
        float h = math.pow((a - b) / (a + b), 2);
        float circumference = math.PI * (a + b) * (1 + (3 * h) / (10 + math.sqrt(4 - 3 * h)));

        // Calculate the total number of points
        int pointCount = math.clamp(Mathf.CeilToInt(circumference / gap), 1, maxPoints);

        // Generate points
        for (int i = 0; i < pointCount; i++)
        {
            float angle = (2 * Mathf.PI * i) / pointCount; // Angle in radians
            float x = a * Mathf.Cos(angle);
            float y = b * Mathf.Sin(angle);

            var center = new float3(ropeCenter.x, ropeCenter.y, 0);

            points.Add(center + new float3(x, y, 0)); // Assuming 2D plane (XY)
        }
    }
}
