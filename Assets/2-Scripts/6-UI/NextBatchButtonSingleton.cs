using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextBatchButtonSingleton : MonoBehaviour
{
    public static NextBatchButtonSingleton Instance;
    
    public string nextSceneName;
    public Button nextBatchButton;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    
    public void Start()
    {
        nextBatchButton.GetComponent<Button>().onClick.AddListener(LoadNextBatch);
    }
    
    public void LoadNextBatch()
    {
        if (nextSceneName.Length == 0)
            return;

        PlayerPrefs.SetInt(nextSceneName, 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene(nextSceneName);
    }
}
