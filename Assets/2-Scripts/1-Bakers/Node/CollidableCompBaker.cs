using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class CollidableCompBaker : MonoBehaviour
{
    public bool isTrigger;
    public Transform tempRadiusTrfm;

    public class Baker : Baker<CollidableCompBaker>
    {
        public override void Bake(CollidableCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            float _radius = authoring.tempRadiusTrfm.transform.lossyScale.x / 2;

            AddComponent(entity, new CollidableComp
            {
                isTrigger = authoring.isTrigger,
                radius = _radius
            });

            AddBuffer<NodeRopeEntityBuffer>(entity);
        }
    }
}