using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class RopePhysicsCompBaker : MonoBehaviour
{
    public bool applyConst;
    public int iterCount;
    public float3 gravity;

    public class Baker : Baker<RopePhysicsCompBaker>
    {
        public override void Bake(RopePhysicsCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity,
                new RopePhysComp
                {
                    applyConst = authoring.applyConst,
                    iterCount = authoring.iterCount,
                    gravity = authoring.gravity,
                });
        }
    }
}
