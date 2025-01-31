using Unity.Entities;
using UnityEngine;

class SpikeTagBaker : MonoBehaviour
{
    class Baker : Baker<SpikeTagBaker>
    {
        public override void Bake(SpikeTagBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<SpikeTag>(entity);
        }
    }
}
