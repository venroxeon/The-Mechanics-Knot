using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class NodeRotatorCompBaker : MonoBehaviour
{
    public CollType type;
    public int targetRotDir;
    public float rotAngleInDegrees;

    public class Baker : Baker<NodeRotatorCompBaker>
    {
        public override void Bake(NodeRotatorCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new NodeRotatorComp()
            {
                type = authoring.type,
                rotAngleUnscaled = math.radians(authoring.rotAngleInDegrees) * authoring.targetRotDir
            });

            AddComponent<DeadlockSimulationDataComp>(entity);
        }
    }
}

