using Unity.Entities;
using UnityEngine;

class GameObjectRefCompBaker : MonoBehaviour
{
    class Baker : Baker<GameObjectRefCompBaker>
    {
        public override void Bake(GameObjectRefCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity,
                new GameObjectRefComp()
                {
                    objRef = authoring.gameObject
                });
        }
    }
}
