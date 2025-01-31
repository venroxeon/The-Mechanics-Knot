using Unity.Entities;
using UnityEngine;

class RotateAroundNodeCompBaker : MonoBehaviour
{
    public float rotateAroundSpeed;
    public GameObject pivotObj;

    class Baker : Baker<RotateAroundNodeCompBaker>
    {
        public override void Bake(RotateAroundNodeCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent<RotateAroundNodeComp>(entity, new()
            {
                rotateAroundSpeed = authoring.rotateAroundSpeed,
                pivotEntity = GetEntity(authoring.pivotObj, TransformUsageFlags.None),
                pivotPos = authoring.pivotObj.transform.position
            });
        }
    }
}
