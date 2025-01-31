using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoadingScreen : MonoBehaviour
{
    VisualElement root, loadingBar;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        loadingBar = root.Q("LoadingBar");

        StartCoroutine(LoadAsynchronously("Scene1"));
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        yield return new WaitForSeconds(0.1f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress);

            StyleLength styleLen = loadingBar.style.maxWidth;
            styleLen.value = progress * 560;
            loadingBar.style.maxWidth = styleLen;

            yield return null;
        }
    }
}
