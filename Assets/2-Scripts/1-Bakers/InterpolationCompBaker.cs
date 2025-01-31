using Unity.Entities;
using UnityEngine;

class InterpolationCompBaker : MonoBehaviour
{
    [Header("Properties To Interpolate")]
    public bool Position;
    public bool Rotation;
    public bool Scale;

    class Baker : Baker<InterpolationCompBaker>
    {
        public override void Bake(InterpolationCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent<InterpolationComp>(entity, new()
            {
                forPosition = authoring.Position,
                forRotation = authoring.Rotation,
                forScale = authoring.Scale
            });
        }
    }
}
