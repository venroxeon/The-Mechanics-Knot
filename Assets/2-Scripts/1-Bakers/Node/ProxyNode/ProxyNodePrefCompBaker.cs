using Unity.Entities;
using UnityEngine;

class ProxyNodePrefCompBaker : MonoBehaviour
{
    public GameObject proxyNodePref;

    class Baker : Baker<ProxyNodePrefCompBaker>
    {
        public override void Bake(ProxyNodePrefCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new ProxyNodePrefEntityComp()
            {
                proxyNodePref = GetEntity(authoring.proxyNodePref, TransformUsageFlags.None)
            });
        }
    }
}
