using Unity.Entities;
using UnityEngine;

class LevelDataForProxyNodeCompBaker : MonoBehaviour
{
    public int maxProxyNodeCount;
    class Baker : Baker<LevelDataForProxyNodeCompBaker>
    {
        public override void Bake(LevelDataForProxyNodeCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<LevelDataForProxyNodeComp>(entity, new()
            {
                maxProxyNodeCount = authoring.maxProxyNodeCount,
                curProxyNodeCount = 0
            });
        }
    }
}
