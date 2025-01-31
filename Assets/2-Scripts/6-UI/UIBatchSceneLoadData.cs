using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBatchSceneLoadData : MonoBehaviour
{
    public string sceneName;

    public void LoadBatch(int levelIndex)
    {
        PlayerPrefs.SetString("CurSceneName", sceneName);
        PlayerPrefs.SetInt(sceneName, levelIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene(sceneName);
    }
}
