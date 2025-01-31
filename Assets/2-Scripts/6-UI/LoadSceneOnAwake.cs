using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnAwake : MonoBehaviour
{
    public int sceneIndex;

    public void Awake()
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }
}
