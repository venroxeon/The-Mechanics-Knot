using Unity.Mathematics;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using CustomUtilities;

[BurstCompile]
public partial struct RopeSegCollisionJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<RopeBasicComp> basicCompLookup;
    [ReadOnly] public ComponentLookup<RopePhysComp> ropePhysCompLookup;
    [ReadOnly] public ComponentLookup<LocalTransform> trfmLookup;
    [ReadOnly] public ComponentLookup<CollidableComp> collCompLookup;
    
    [ReadOnly] public NativeArray<Entity> arrColEntity;

    [ReadOnly] public EntityStorageInfoLookup storageInfoEntity;

    [BurstCompile]
    public void Execute(ref TempLocalTransform tempTrfm, ref RopeSegCollisionComp segCollComp, in RopeSegParEntityComp ropeSegParComp)
    {
        float segRadius = basicCompLookup[ropeSegParComp.parEntity].segRadius;

        Entity colEntity;
        CollidableComp colComp;
        LocalTransform trfm;
        
        float minDist = 999f;
        for(int i = 0; i < arrColEntity.Length; i++)
        {
            if (segCollComp.isCol)
            {
                var entity = segCollComp.closestCollEntity;

                if(storageInfoEntity.Exists(entity))
                {
                    colComp = collCompLookup.GetRefRO(entity).ValueRO;

                    trfm = trfmLookup.GetRefRO(entity).ValueRO;

                    RopePhysicsUtilities.CheckDistAndRespond(colComp.isTrigger, segRadius, trfm.Position, colComp.radius, ref tempTrfm, ref segCollComp);

                    break;
                }
                else
                {
                    segCollComp.isCol = false;
                }
            }

            colEntity = arrColEntity[i];
            colComp = collCompLookup.GetRefRO(colEntity).ValueRO;
            trfm = trfmLookup.GetRefRO(colEntity).ValueRO;

            if (RopePhysicsUtilities.CheckDistAndRespond(colComp.isTrigger, out float segToColDist, segRadius, trfm.Position, colComp.radius, ref tempTrfm, ref segCollComp))
            {
                segCollComp.closestCollEntity = colEntity;

                break;
            }
            else
            {
                if (segToColDist < minDist)
                {
                    segCollComp.closestCollEntity = colEntity;
                    minDist = segToColDist;
                }
            }
        }
    }
}