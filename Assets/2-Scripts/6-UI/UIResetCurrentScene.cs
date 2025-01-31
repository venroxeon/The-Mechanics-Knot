using UnityEngine;
using UnityEngine.SceneManagement;

public class UIResetCurrentScene : MonoBehaviour
{
    public void ResetBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
