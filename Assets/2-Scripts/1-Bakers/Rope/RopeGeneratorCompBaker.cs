using Unity.Entities;
using UnityEngine;

public class RopeGeneratorCompBaker : MonoBehaviour
{
    public class Baker : Baker<RopeGeneratorCompBaker>
    {
        public override void Bake(RopeGeneratorCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new RopeGeneratorComp());
        }
    }
}
