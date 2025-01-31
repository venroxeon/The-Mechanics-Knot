using UnityEngine;
using TMPro;
using Unity.Entities;
public class UIRopeCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ropeCounterText;

    public void Update()
    {
        EntityManager EM = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQueryBuilder builder = new(Unity.Collections.Allocator.Temp);
        builder.WithAll<LevelManagerDataComp>();

        EntityQuery query = builder.Build(EM);

        if(query.HasSingleton<LevelManagerDataComp>())
        {
            var levelManComp = query.GetSingleton<LevelManagerDataComp>();

            var levelData = EM.GetComponentData<LevelDataComp>(levelManComp.curLevelEntity);

            ropeCounterText.text = (levelData.ropeCountLimit - levelData.curRopeCount).ToString();
        }

        builder.Dispose();
    }
}
