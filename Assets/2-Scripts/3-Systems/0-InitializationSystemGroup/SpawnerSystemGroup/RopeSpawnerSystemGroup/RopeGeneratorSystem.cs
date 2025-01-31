using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[BurstCompile]
public struct NextSegPosDir
{
    public int noOfSeg;
    public float3 delta;
}

[UpdateInGroup(typeof(RopeSpawnerSystemGroup))]
[UpdateAfter(typeof(RopeSpawnerSystem))]
[BurstCompile]
public partial struct RopeGeneratorSystem : ISystem
{
    bool clearList;
    NativeList<Entity> listRopeSeg;
    EntityQuery ropeQuery;
    ComponentTypeSet trfmTypeSet;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ropeQuery = QueryBuilder().WithAll<
            RopeGeneratorComp, 
            RopeBasicComp, 
            RopeSegEntityBuffer>().Build();
        
        state.RequireForUpdate(ropeQuery);

        listRopeSeg = new(Allocator.Persistent);

        trfmTypeSet = new(ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadWrite<LocalToWorld>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        InputComp input = GetSingleton<InputComp>();

        GenerateRope(input.isFirstTouchPress, input.isSecTouchPress, in input.firstTouchPos, ref state);
    }

    [BurstCompile]
    void GenerateRope(in bool isFirstTouchPress, in bool isSecTouchPress, in float3 firstTouchPos, ref SystemState state)
    {
        EntityCommandBuffer ECB = new(Allocator.TempJob);

        foreach (var (_ropeGenComp, _ropeBasicComp, segBuffer, linkedEntityBuffer, ropeEntity) in Query<
            RefRW<RopeGeneratorComp>, 
            RefRO<RopeBasicComp>, 
            DynamicBuffer<RopeSegEntityBuffer>,
            DynamicBuffer<LinkedEntityGroup>>()
            .WithEntityAccess())
        {
            //GETTER
            RopeGeneratorComp ropeGenComp = _ropeGenComp.ValueRO;
            RopeBasicComp ropeBasicComp = _ropeBasicComp.ValueRO;
            //GETTER

            if(isFirstTouchPress && isSecTouchPress)
            {
                ECB.AddComponent<DestroyRopeWithoutDecrementTag>(ropeEntity);
                clearList = true;
            }
            else if (isFirstTouchPress && segBuffer.Length < ropeBasicComp.segLimit)
            {
                float segDist = ropeBasicComp.segDist;
                float3 lastSegPos = ropeGenComp.lastSegPos;

                if (lastSegPos.x == firstTouchPos.x && lastSegPos.y == firstTouchPos.y) continue;

                NextSegPosDir next = GetDelta(in segDist, in ropeGenComp.lastSegPos, in firstTouchPos);

                //LIMIT CHECK
                if (next.noOfSeg == 0) continue;
                if (segBuffer.Length + next.noOfSeg > ropeBasicComp.segLimit)
                {
                    next.noOfSeg = ropeBasicComp.segLimit - segBuffer.Length;
                }

                Entity segPref = ropeBasicComp.segPref;

                NativeArray<Entity> arrSeg = new(next.noOfSeg, Allocator.Temp);
                state.EntityManager.Instantiate(segPref, arrSeg);

                float3 newPos = ropeGenComp.lastSegPos;
                foreach (Entity seg in arrSeg)
                {
                    newPos += next.delta;

                    var tempTrfm = GetComponentRW<TempLocalTransform>(seg);
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
                listRopeSeg.AddRange(arrSeg);

                ropeGenComp.lastSegPos = newPos;

                arrSeg.Dispose();
            }
            else if(!isFirstTouchPress)
            {
                listRopeSeg.Add(ropeEntity);

                ECB.RemoveComponent<RopeGeneratorComp>(ropeEntity);
                ECB.AddComponent<RopeGenerationCompleteTag>(listRopeSeg.AsArray());

                clearList = true;

                //UPDATE ROPE COUNT
                var levelManager = GetSingleton<LevelManagerDataComp>();
                var curLevelData = GetComponent<LevelDataComp>(levelManager.curLevelEntity);

                GetComponentRW<LevelDataComp>(levelManager.curLevelEntity).ValueRW.curRopeCount++;
            }

            //SETTER
            _ropeGenComp.ValueRW = ropeGenComp;
        }

        if (clearList)
        {
            listRopeSeg.Clear();
            clearList = false;
        }

        if(!ECB.IsEmpty)
            ECB.Playback(state.EntityManager);

        ECB.Dispose();
    }

    [BurstCompile]
    NextSegPosDir GetDelta(in float segDist, in float3 lastSegPos, in float3 touchPos)
    {
        float dist;
        float3 delta;
        NextSegPosDir next;

        delta = touchPos - lastSegPos;
        dist = math.distance(touchPos, lastSegPos);
        delta /= dist;

        next.noOfSeg = (int)(dist / segDist);
        next.delta = delta * segDist;
        
        return next;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        listRopeSeg.Dispose();
    }
}
