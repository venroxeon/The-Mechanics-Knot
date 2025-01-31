using Unity.Entities;
using UnityEngine;

public class InputCompBaker : MonoBehaviour
{
    public class Baker : Baker<InputCompBaker>
    {
        public override void Bake(InputCompBaker auth)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent<InputComp>(entity);
        }
    }
}