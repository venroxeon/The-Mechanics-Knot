using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class PreRenderedRopeCompBaker : MonoBehaviour
{
    [SerializeField] int levelIndex, segCount;
    [SerializeField] float2 ropeCenter;
    [SerializeField] float semiMajor, semiMinor, segDist;
    class Baker : Baker<PreRenderedRopeCompBaker>
    {
        public override void Bake(PreRenderedRopeCompBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<PreRenderedRopeComp>(entity, new()
            {
                levelIndex = authoring.levelIndex,
                segCount = authoring.segCount,
                ropeCenter = authoring.ropeCenter,
                semiMajor = authoring.semiMajor,
                semiMinor = authoring.semiMinor,
                segDist = authoring.segDist
            });
        }
    }
}
