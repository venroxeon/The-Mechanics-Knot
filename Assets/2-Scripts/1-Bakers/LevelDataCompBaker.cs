using Unity.Entities;
using UnityEngine;

class LevelDataCompBaker : MonoBehaviour
{
    public int ropeCountLimit;

    class Baker : Baker<LevelDataCompBaker>
    {
        public override void Bake(LevelDataCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<LevelDataComp>(entity, new()
            {
                ropeCountLimit = authoring.ropeCountLimit
            });
        }
    }
}
