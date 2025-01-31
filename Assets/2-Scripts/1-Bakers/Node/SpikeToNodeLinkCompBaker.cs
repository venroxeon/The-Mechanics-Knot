using Unity.Entities;
using UnityEngine;

class SpikeToNodeLinkCompBaker : MonoBehaviour
{
    public GameObject linkedNode;

    class Baker : Baker<SpikeToNodeLinkCompBaker>
    {
        public override void Bake(SpikeToNodeLinkCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent<SpikeToNodeLinkComp>(entity, new()
            {
                isActive = false,
                linkedNode = GetEntity(authoring.linkedNode, TransformUsageFlags.Dynamic)
            });
        }
    }
}
