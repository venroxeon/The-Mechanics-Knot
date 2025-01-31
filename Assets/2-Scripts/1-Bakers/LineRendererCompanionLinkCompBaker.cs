using Unity.Entities;
using UnityEngine;

class LineRendererCompanionLinkCompBaker : MonoBehaviour
{
    public GameObject lineRenPref;

    [Header("SCRIPT ACCESS")]
    public RopeBasicCompBaker basicCompBaker;

    class Baker : Baker<LineRendererCompanionLinkCompBaker>
    {
        public override void Bake(LineRendererCompanionLinkCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new LineRendererCompanionLinkComp
            {
                isPrefSpawned = false,
                lineRenPref = authoring.lineRenPref
            });

            var buffer = AddBuffer<RopeSegPosBuffer>(entity);
            buffer.EnsureCapacity(authoring.basicCompBaker.segLimit + 2);
            buffer.Length = authoring.basicCompBaker.segLimit + 2;
        }
    }
}

