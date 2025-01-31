using TMPro;
using Unity.Entities;
using UnityEngine;

public class UIProxyNodeCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI proxyNodeCounterText;
    public void Update()
    {
        EntityManager EM = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQueryBuilder builder = new(Unity.Collections.Allocator.Temp);
        builder.WithAll<LevelManagerDataComp>();

        EntityQuery query = builder.Build(EM);

        if (query.HasSingleton<LevelManagerDataComp>())
        {
            var levelManComp = query.GetSingleton<LevelManagerDataComp>();

            var proxyNodeData = EM.GetComponentData<LevelDataForProxyNodeComp>(levelManComp.curLevelEntity);

            proxyNodeCounterText.text = (proxyNodeData.maxProxyNodeCount - proxyNodeData.curProxyNodeCount).ToString();
        }

        builder.Dispose();
    }
}
