using Unity.Entities;
using UnityEngine;

class DisableOnRunTagBaker : MonoBehaviour
{
    class Baker : Baker<DisableOnRunTagBaker>
    {
        public override void Bake(DisableOnRunTagBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<DisableOnRunTag>(entity);
        }
    }
}
