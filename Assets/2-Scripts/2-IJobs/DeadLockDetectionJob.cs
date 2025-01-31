using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using System;
using Unity.Transforms;
using Unity.Jobs;

[BurstCompile]
public partial struct DeadLockDetectionJob : IJob
{
    public NativeReference<bool> hasDeadlock;

    public NativeHashSet<Entity> hashSetProcessedRopes;
    public NativeHashSet<NodeData> hashSetProcessedNodes;
    public NativeQueue<NodeData> queueMainNodes;

    public ComponentLookup<RopeSegVelocityComp> segVelCompLookup;
    public ComponentLookup<DeadlockSimulationDataComp> deadLockSimCompLookup;
    public ComponentLookup<NodeRotatorComp> nodeRotCompLookup;

    [ReadOnly] public NativeArray<Entity> arrNodes;

    [ReadOnly] public ComponentLookup<RopeSegCollisionComp> segCollCompLookup;
    [ReadOnly] public ComponentLookup<TempLocalTransform> tempTrfmLookup;
    [ReadOnly] public ComponentLookup<LocalTransform> trfmLookup;

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
        //ADD MAIN NODES TO QUEUE AND PROCESSED NODES(HASH SET)

        foreach (var node in arrNodes)
        {
            var nodeRotComp = nodeRotCompLookup[node];
            
            if (nodeRotComp.type == CollType.Main)
            {
                var nodeData = new NodeData()
                {
                    nodeEntity = node,
                    nodeRootMainNodeEntity = node
                };

                //assignedDir = nodeRotComp.targetRotDir;

                queueMainNodes.Enqueue(nodeData);
                hashSetProcessedNodes.Add(nodeData);
            }

            deadLockSimCompLookup[node] = new()
            {
                assignedDir = 1,
                isAssigned = false
            };
        }

        //MAIN PROCESSING
        while (!queueMainNodes.IsEmpty())
        {
            var curNodeData = queueMainNodes.Dequeue();

            //PROCESS EACH ROPE IN NODE
            foreach (var ropeElem in nodeRopeBufferLookup[curNodeData.nodeEntity])
            {
                //ASSIGN DIRECTION IF ROPE NOT PROCESSED
                if (!hashSetProcessedRopes.Contains(ropeElem.ropeEntity))
                {
                    var segBuffer = ropeSegBufferLookup[ropeElem.ropeEntity];

                    if (TryGetSegIndexCollWithNode(out int startIndex, curNodeData.nodeEntity, segBuffer))
                    {
                        AssignNodeDirection(startIndex, segBuffer);
                    }

                    hashSetProcessedRopes.Add(ropeElem.ropeEntity);
                }

                //ADD CONNECTED NODES IF NOT PROCESSED
                foreach (var nodeElem in ropeNodeBufferLookup[ropeElem.ropeEntity])
                {
                    if (nodeElem.nodeEntity == curNodeData.nodeEntity)
                        continue;

                    NodeData nodeData = new()
                    {
                        nodeEntity = nodeElem.nodeEntity,
                        nodeRootMainNodeEntity = curNodeData.nodeRootMainNodeEntity
                    };

                    if (!hashSetProcessedNodes.Contains(nodeData) && deadLockSimCompLookup.HasComponent(nodeData.nodeEntity))
                    {
                        queueMainNodes.Enqueue(nodeData);
                    }
                }
            }

            hashSetProcessedNodes.Add(curNodeData);
        }

        foreach(var nodeData in hashSetProcessedNodes)
        {
            if (deadLockSimCompLookup[nodeData.nodeEntity].assignedDir == 0)
            {
                if(nodeRotCompLookup.HasComponent(nodeData.nodeRootMainNodeEntity))
                {
                    nodeRotCompLookup.GetRefRW(nodeData.nodeRootMainNodeEntity).ValueRW.isInDeadLock = true;

                    DisableVelocityOnRopes(nodeData.nodeEntity);

                    hasDeadlock.Value = true;
                }
            }
        }
    }

    [BurstCompile]
    void AssignNodeDirection(in int startIndex, in DynamicBuffer<RopeSegEntityBuffer> segBuffer)
    {
        bool isFindingSecPoint = false;
        float3 prevPoint = default, curPoint;
        Entity prevNodeEntity = default, curNodeEntity = default;

        for (int k = 0; k <= segBuffer.Length; k++)
        {
            int i = (startIndex + k) % segBuffer.Length;
            int j = (i + 1) % segBuffer.Length;

            var firstSegCollComp = segCollCompLookup[segBuffer[i].segEntity];
            var secSegCollComp = segCollCompLookup[segBuffer[j].segEntity];

            if (!isFindingSecPoint && !secSegCollComp.isCol)
            {
                prevPoint = tempTrfmLookup[segBuffer[i].segEntity].Position;
                prevNodeEntity = firstSegCollComp.closestCollEntity;

                isFindingSecPoint = true;
            }
            else if(isFindingSecPoint && secSegCollComp.isCol && deadLockSimCompLookup.HasComponent(secSegCollComp.closestCollEntity))
            {
                curPoint = tempTrfmLookup[segBuffer[j].segEntity].Position;
                curNodeEntity = secSegCollComp.closestCollEntity;

                var prevNodeSimData = deadLockSimCompLookup[prevNodeEntity];
                var curNodeSimData = deadLockSimCompLookup[curNodeEntity];

                var rotDir = GetRotDirection(prevNodeSimData, curNodeSimData, prevNodeEntity, curNodeEntity, prevPoint, curPoint);

                curNodeSimData.assignedDir = rotDir;
                curNodeSimData.isAssigned = true;

                deadLockSimCompLookup[curNodeEntity] = curNodeSimData;

                isFindingSecPoint = false;
            }
        }
    }

    [BurstCompile]
    int GetRotDirection(in DeadlockSimulationDataComp prevNodeRot, in DeadlockSimulationDataComp curNodeRot, in Entity node1, in Entity node2, in float3 point1, in float3 point2)
    {
        if (prevNodeRot.isAssigned && prevNodeRot.assignedDir == 0) return 0;

        int rotToAssign;

        if(CustomUtilities.Utilities.DoIntersect(trfmLookup[node1].Position, trfmLookup[node2].Position, point1, point2))
        {
            rotToAssign = -prevNodeRot.assignedDir;
        }
        else
        {
            rotToAssign = prevNodeRot.assignedDir;
        }

        if (curNodeRot.isAssigned && curNodeRot.assignedDir != rotToAssign)
        {
            rotToAssign = 0;
        }

        return rotToAssign;
    }

    [BurstCompile]
    bool TryGetSegIndexCollWithNode(out int index, in Entity node, in DynamicBuffer<RopeSegEntityBuffer> segBuffer)
    {
        bool isFound = false;

        int prevIndex = 0;
        index = 0;

        for(int k = 0; k <= segBuffer.Length; k++)
        {
            int i = k % segBuffer.Length;

            var seg = segBuffer[i];

            var segCollComp = segCollCompLookup[seg.segEntity];

            if (isFound && !segCollComp.isCol)
            {
                index = prevIndex;
                return true;
            }
            else if (segCollComp.isCol && segCollComp.closestCollEntity == node)
            {
                isFound = true;
            }

            prevIndex = i;
        }

        return false;
    }

    [BurstCompile]
    void DisableVelocityOnRopes(in Entity node)
    {
        foreach(var ropeElem in nodeRopeBufferLookup[node])
        {
            foreach(var segElem in ropeSegBufferLookup[ropeElem.ropeEntity])
            {
                if (segCollCompLookup[segElem.segEntity].isCol)
                    segVelCompLookup.GetRefRW(segElem.segEntity).ValueRW.hasVelocity = false;
            }
        }
    }
}

[BurstCompile]
public struct NodeData : IEquatable<NodeData>
{
    public Entity nodeEntity, nodeRootMainNodeEntity;

    public bool Equals(NodeData other)
    {
        return nodeEntity == other.nodeEntity && nodeRootMainNodeEntity == other.nodeRootMainNodeEntity;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + nodeEntity.GetHashCode();
            hash = hash * 31 + nodeRootMainNodeEntity.GetHashCode();
            
            return hash;
        }
    }
}