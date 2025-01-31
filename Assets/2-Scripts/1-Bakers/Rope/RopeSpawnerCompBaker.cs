using Unity.Entities;
using UnityEngine;

public class RopeSpawnerCompBaker : MonoBehaviour
{
    [SerializeField] GameObject ropePref;

    public class Baker : Baker<RopeSpawnerCompBaker>
    {
        public override void Bake(RopeSpawnerCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity,
                new RopeSpawnerComp
                {
                    ropePref = GetEntity(authoring.ropePref, TransformUsageFlags.None)
                });
        }
    }
}
