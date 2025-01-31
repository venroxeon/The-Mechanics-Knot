using Unity.Entities;
using UnityEngine;

class FixedStepSimulationParamCompBaker : MonoBehaviour
{
    public float timeStep;

    class Baker : Baker<FixedStepSimulationParamCompBaker>
    {
        public override void Bake(FixedStepSimulationParamCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<FixedStepSimulationParamComp>(entity, new()
            {
                fixedDeltaTime = authoring.timeStep
            });
        }
    }
}
