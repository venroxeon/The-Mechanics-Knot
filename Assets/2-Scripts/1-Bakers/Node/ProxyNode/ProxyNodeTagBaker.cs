using Unity.Entities;
using UnityEngine;

class ProxyNodeTagBaker : MonoBehaviour
{
    class Baker : Baker<ProxyNodeTagBaker>
    {
        public override void Bake(ProxyNodeTagBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<ProxyNodeTag>(entity);
        }
    }
}
