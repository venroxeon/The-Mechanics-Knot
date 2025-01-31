using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoadSaveState : MonoBehaviour
{
    [SerializeField] string newGameSceneName;

    public void LoadSaveState()
    {
        if(!PlayerPrefs.HasKey("CurSceneName"))
        {
            NewGame();
        }
        else
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("CurSceneName"));
        }
    }

    public void NewGame()
    {
        PlayerPrefs.SetString("CurSceneName", newGameSceneName);
        PlayerPrefs.SetInt(newGameSceneName, 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene(newGameSceneName);
    }
}
