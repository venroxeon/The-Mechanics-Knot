using Unity.Entities;
using UnityEngine;

public class RopeBasicCompBaker : MonoBehaviour
{
    public int segLimit;

    public float segDist, segRadius;
    
    public GameObject segPref;
    public class Baker : Baker<RopeBasicCompBaker>
    {
        public override void Bake(RopeBasicCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity,
                new RopeBasicComp
                {
                    segLimit = authoring.segLimit,
                    segDist = authoring.segDist,
                    segDistSqr = authoring.segDist * authoring.segDist,
                    segRadius = authoring.segRadius,
                    segPref = GetEntity(authoring.segPref, TransformUsageFlags.None)
                });
            AddBuffer<RopeSegEntityBuffer>(entity);
            AddBuffer<RopeNodeEntityBuffer>(entity);
        }
    }
}