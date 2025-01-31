using Unity.Entities;
using UnityEngine;

public class RopeSegPhysCompBaker : MonoBehaviour
{
    public bool hasVelocity;

    public class Baker : Baker<RopeSegPhysCompBaker>
    {
        public override void Bake(RopeSegPhysCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent<RopeSegVelocityComp>(entity, new()
            {
                hasVelocity = authoring.hasVelocity
            });
            AddComponent<RopeSegMovementComp>(entity);
            AddComponent<RopeSegCollisionComp>(entity);
            AddComponent<RopeSegParEntityComp>(entity);
        }
    }
}