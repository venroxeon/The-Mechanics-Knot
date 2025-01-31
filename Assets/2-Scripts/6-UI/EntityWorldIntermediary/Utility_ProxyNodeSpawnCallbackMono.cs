using Unity.Entities;
using UnityEngine;

public class Utility_ProxyNodeSpawnCallbackMono : MonoBehaviour
{
    bool isTouchActive;
    Entity activeEntity;
    
    public UIUtilityListButtonCallBacks utilityButtonCallback;

    public void Start()
    {
        utilityButtonCallback.onUtilityButtonDragged += OnDragAddComp;
        utilityButtonCallback.onUtilityButtonReleased += OnReleased;
    }

    void OnDragAddComp()
    {
        World world = World.DefaultGameObjectInjectionWorld;
        activeEntity = world.EntityManager.CreateEntity(world.EntityManager.CreateArchetype(typeof(ProxyNodeButtonPressedComp)));

        world.EntityManager.SetComponentData<ProxyNodeButtonPressedComp>(activeEntity, new()
        {
            isTouchActive = true,
            touchPos = InputSystemManager.Instance.GetFirstTouchPos()
        });

        isTouchActive = true;
    }

    void OnReleased()
    {
        if (isTouchActive)
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<ProxyNodeButtonPressedComp>(activeEntity, new()
            {
                isTouchActive = false,
                touchPos = InputSystemManager.Instance.GetFirstTouchPos()
            });
            
            isTouchActive = false;
        }
    }

    public void Update()
    {
        if(isTouchActive)
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<ProxyNodeButtonPressedComp>(activeEntity, new()
            {
                isTouchActive = true,
                touchPos = InputSystemManager.Instance.GetFirstTouchPos()
            });
        }
    }
}
