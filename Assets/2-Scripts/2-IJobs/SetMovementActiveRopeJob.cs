//using Unity.Entities;
//using Unity.Burst;
//using Unity.Collections;

//[BurstCompile]
//public partial struct SetMovementActiveRopeJob : IJobEntity
//{
//    [NativeDisableParallelForRestriction]
//    public ComponentLookup<RopeSegMovementComp> segMoveCompLookup;
    
//    [NativeDisableParallelForRestriction]
//    [ReadOnly]
//    public ComponentLookup<RopeSegCollisionComp> segCollCompLookup;

//    [BurstCompile]
//    public void Execute(in SetMovementActiveRopeComp setMoveComp, in DynamicBuffer<RopeSegEntityBuffer> segBuffer)
//    {
//        foreach (var segElem in segBuffer)
//        {
//            if (segCollCompLookup[segElem.segEntity].isCol)
//                segMoveCompLookup.GetRefRW(segElem.segEntity).ValueRW.hasMovementFromNode = setMoveComp.isActive;
//        }
//    }
//}
