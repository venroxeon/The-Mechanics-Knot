using Unity.Entities;
using UnityEngine;

class CircleCollisionDetectionCompBaker : MonoBehaviour
{
    class Baker : Baker<CircleCollisionDetectionCompBaker>
    {
        public override void Bake(CircleCollisionDetectionCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<CircleCollisionDetectionComp>(entity, new()
            {
                radius = GetComponent<SpriteRenderer>().bounds.extents.x
            });
        }
    }
}
