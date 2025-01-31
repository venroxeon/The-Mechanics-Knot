using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelButtonSingleton : MonoBehaviour
{
    public static NextLevelButtonSingleton Instance;
    
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(AddTag);
    }

    void AddTag()
    {
        var EM = World.DefaultGameObjectInjectionWorld.EntityManager;

        var world = World.DefaultGameObjectInjectionWorld;

        EntityQueryBuilder queryBuilder = new(Allocator.Temp);
        queryBuilder.WithAll<LevelManagerDataComp, LevelBuffer>();

        var levelEntity = queryBuilder.Build(world.EntityManager).GetSingletonEntity();

        world.EntityManager.AddComponent<NextButtonPressedTag>(levelEntity);
    }
}
