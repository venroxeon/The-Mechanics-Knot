using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public partial struct RopeCollidingNodesCollectionJob : IJobFor
{
    public NativeHashSet<Entity> ropeNodeHashSet;

    public BufferLookup<RopeNodeEntityBuffer> ropeNodeEntityBufferLookup;
    public BufferLookup<NodeRopeEntityBuffer> nodeRopeEntityBufferLookup;

    [ReadOnly] public NativeArray<Entity> arrRopes;

    [ReadOnly] public ComponentLookup<RopeSegCollisionComp> segCollCompLookup;

    [ReadOnly] public BufferLookup<RopeSegEntityBuffer> ropeSegEntityBufferLookup;

    [BurstCompile]
    public void Execute(int index)
    {
        ropeNodeHashSet.Clear();

        var ropeEntity = arrRopes[index];
        var segBuffer = ropeSegEntityBufferLookup[ropeEntity];
        var ropeNodeBuffer = ropeNodeEntityBufferLookup[ropeEntity];

        if (segBuffer.Length == 0)
            return;

        Entity prevClosestEntity = default;
        
        for (int i = 0; i < segBuffer.Length; i++)
        {
            var segPhysComp = segCollCompLookup[segBuffer[i].segEntity];

            if (!segPhysComp.isCol)
                continue;

            if (segPhysComp.closestCollEntity == prevClosestEntity)
                continue;

            if (!ropeNodeHashSet.Contains(segPhysComp.closestCollEntity))
            {
                ropeNodeHashSet.Add(segPhysComp.closestCollEntity);

                ropeNodeBuffer.Add(new()
                {
                    nodeEntity = segPhysComp.closestCollEntity
                });
            }
            ///
            prevClosestEntity = segPhysComp.closestCollEntity;

            //NodeRopeBuffer Check Duplicates and Add

            bool check = false;
            var nodeRopeBuffer = nodeRopeEntityBufferLookup[segPhysComp.closestCollEntity];
            foreach (var elem in nodeRopeBuffer)
            {
                if (elem.ropeEntity == ropeEntity)
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                nodeRopeBuffer.Add(new()
                {
                    ropeEntity = ropeEntity
                });
            }
            ///
        }
    }
}
