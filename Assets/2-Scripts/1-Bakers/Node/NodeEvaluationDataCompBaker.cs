using Unity.Entities;
using UnityEngine;

class NodeEvaluationDataCompBaker : MonoBehaviour
{
    public int targetRotDir; 

    class Baker : Baker<NodeEvaluationDataCompBaker>
    {
        public override void Bake(NodeEvaluationDataCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<NodeEvaluationDataComp>(entity, new()
            {
                targetRotDir = authoring.targetRotDir
            });
        }
    }
}
