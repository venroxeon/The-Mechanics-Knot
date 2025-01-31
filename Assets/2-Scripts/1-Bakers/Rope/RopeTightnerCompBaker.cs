using Unity.Entities;
using UnityEngine;

class RopeTightnerCompBaker : MonoBehaviour
{
    public int removeCount;
    public float extraLenMin, extraLenMax, reelDelayDuration;

    class RopeTightnerCompBakerBaker : Baker<RopeTightnerCompBaker>
    {
        public override void Bake(RopeTightnerCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new RopeTightnerComp 
            { 
                removeCount = authoring.removeCount,
                extraLenMin = authoring.extraLenMin,
                extraLenMax = authoring.extraLenMax,
                reelDelayDuration = authoring.reelDelayDuration, 
                curTotalTime = 0 
            });
        }
    }
}

