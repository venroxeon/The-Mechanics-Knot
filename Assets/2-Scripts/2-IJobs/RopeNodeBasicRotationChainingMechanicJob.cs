using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;


[BurstCompile]
public partial struct RopeNodeBasicRotationChainingMechanicJob : IJob
{
    public NativeQueue<Entity> queueMainNodes;
    public NativeHashSet<Entity> processedRopes;

    public ComponentLookup<NodeRotatorComp> nodeRotCompLookup;

    public ComponentLookup<RopeSegVelocityComp> segVelCompLookup;
    public ComponentLookup<RopeSegMovementComp> segMoveCompLookup;

    [ReadOnly] public NativeArray<Entity> arrNodes;

    [ReadOnly] public ComponentLookup<RopeSegCollisionComp> segCollCompLookup;
    
    [ReadOnly] public BufferLookup<RopeNodeEntityBuffer> ropeNodeBufferLookup;
    [ReadOnly] public BufferLookup<NodeRopeEntityBuffer> nodeRopeBufferLookup;
    [ReadOnly] public BufferLookup<RopeSegEntityBuffer> ropeSegBufferLookup;

    [BurstCompile]
    public void Execute()
    {
        Mechanic();
    }

    [BurstCompile]
    public void Mechanic()
    {
        foreach (var node in arrNodes)
        {
            if (nodeRotCompLookup[node].type == CollType.Main)
            {
                queueMainNodes.Enqueue(node);
            }
        }

        while (!queueMainNodes.IsEmpty())
        {
            var curNode = queueMainNodes.Dequeue();
            //var angle = nodeRotCompLookup[curNode].rotAngleUnscaled;

            foreach (var ropeElem in nodeRopeBufferLookup[curNode])
            {
                if (!processedRopes.Contains(ropeElem.ropeEntity))
                {
                    processedRopes.Add(ropeElem.ropeEntity);
                }
                else
                    continue;

                foreach (var segElem in ropeSegBufferLookup[ropeElem.ropeEntity])
                {
                    var segVelComp = segVelCompLookup[segElem.segEntity];
                    var segCollComp = segCollCompLookup[segElem.segEntity];
                    var segMoveComp = segMoveCompLookup[segElem.segEntity];

                    if (segCollComp.isCol && nodeRotCompLookup.HasComponent(segCollComp.closestCollEntity))
                    {
                        if (nodeRotCompLookup[segCollComp.closestCollEntity].isRotating)
                        {
                            segVelComp.hasVelocity = false;
                            segMoveComp.hasMovementFromNode = true;
                            segMoveComp.shouldRotateNode = false;
                        }
                        else
                        {
                            segMoveComp.shouldRotateNode = true;
                            segVelComp.hasVelocity = true;
                            segMoveComp.hasMovementFromNode = false;
                        }
                    }
                    else
                    {
                        segVelComp.hasVelocity = true;
                        segMoveComp.hasMovementFromNode = false;
                        segMoveComp.shouldRotateNode = false;
                    }

                    segVelCompLookup[segElem.segEntity] = segVelComp;
                    segMoveCompLookup[segElem.segEntity] = segMoveComp;
                }


                foreach (var nodeElem in ropeNodeBufferLookup[ropeElem.ropeEntity])
                {
                    if (nodeElem.nodeEntity == curNode)
                        continue;

                    queueMainNodes.Enqueue(nodeElem.nodeEntity);

                    //var nodeRotComp = nodeRotCompLookup[nodeElem.nodeEntity];
                }

                processedRopes.Add(ropeElem.ropeEntity);

            }

            //ASSIGN IS ROTATING FLAG AFTER PROCESSING ALL ROPES ON THE CURRENT NODE
            foreach (var ropeElem in nodeRopeBufferLookup[curNode])
            {
                foreach (var nodeElem in ropeNodeBufferLookup[ropeElem.ropeEntity])
                {
                    if (nodeElem.nodeEntity == curNode)
                        continue;

                    if(nodeRotCompLookup.HasComponent(nodeElem.nodeEntity))
                        nodeRotCompLookup.GetRefRW(nodeElem.nodeEntity).ValueRW.isRotating = true;
                }

            }
        }
    }
}
