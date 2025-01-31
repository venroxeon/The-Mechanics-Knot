using System.Collections;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandAnimation : MonoBehaviour
{
    bool hasStartedAnimation = false;
    float curTimeTotal = 0;

    [SerializeField] float delay;
    [SerializeField] Animator handAnimator;
    [SerializeField] TextMeshProUGUI helpText;

    public void Start()
    {
        handAnimator.gameObject.SetActive(false);
    }

    public void Update()
    {
        curTimeTotal += Time.deltaTime;

        if (curTimeTotal < delay)
            return;

        int levelIndex = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0);

        if (levelIndex == 0)
        {
            if(!HasSpawnedRope() && !InputSystemManager.Instance.isFirstTouchPress && !hasStartedAnimation)
            {
                handAnimator.gameObject.SetActive(true);
                handAnimator.Play("HandAnimation");
                hasStartedAnimation = true;
            }
            else if (HasSpawnedRope() || InputSystemManager.Instance.isFirstTouchPress)
            {
                handAnimator.gameObject.SetActive(false);
                hasStartedAnimation = false;
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    bool HasSpawnedRope()
    {
        EntityManager EM = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQueryBuilder builder = new(Unity.Collections.Allocator.Temp);
        builder.WithAll<LevelManagerDataComp>();

        EntityQuery query = builder.Build(EM);

        int curRopeCount = 0;

        if (query.HasSingleton<LevelManagerDataComp>())
        {
            var levelManComp = query.GetSingleton<LevelManagerDataComp>();

            var levelData = EM.GetComponentData<LevelDataComp>(levelManComp.curLevelEntity);

            curRopeCount = levelData.curRopeCount;
        }

        builder.Dispose();

        return curRopeCount != 0 ? true : false;
    }
}
