using Unity.Entities;
using UnityEngine;

class LevelManagerBaker : MonoBehaviour
{
    public int curLevelIndex;
    public float nextLevelEnableTime, curLevelDisableTime;

    class Baker : Baker<LevelManagerBaker>
    {
        public override void Bake(LevelManagerBaker authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            
            var levelBuffer = AddBuffer<LevelBuffer>(entity);

            for (int i = 0; i < authoring.transform.childCount; i++)
            {
                levelBuffer.Add(new()
                {
                    levelEntity = GetEntity(authoring.transform.GetChild(i).gameObject, TransformUsageFlags.None)
                });
            }

            AddComponent<LevelManagerDataComp>(entity, new()
            {
                curLevelIndex = authoring.curLevelIndex,
                curLevelEntity = levelBuffer[authoring.curLevelIndex].levelEntity,
                curLevelDisableTime = authoring.curLevelDisableTime,
                nextLevelEnableTime = authoring.nextLevelEnableTime
            });

            AddComponent<JumpToLevelComp>(entity, new()
            {
                targetLevelIndex = authoring.curLevelIndex
            });
        }
    }
}
