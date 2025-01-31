using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

class CustomTransformBaker : MonoBehaviour
{
    public bool addTempTransform;
    public TransformUsageFlags trfmUsageFlags;
    
    class Baker : Baker<CustomTransformBaker>
    {
        public override void Bake(CustomTransformBaker authoring)
        {
            if(authoring.addTempTransform)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<TempLocalTransform>(entity, new()
                {
                    Position = authoring.transform.localPosition,
                    Rotation = authoring.transform.localRotation,
                    Scale = authoring.transform.localScale.x
                });
            }
            else
            {
                GetEntity(authoring.trfmUsageFlags);
            }
        }
    }
}