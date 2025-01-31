using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPauseMenu : MonoBehaviour
{
    public static UIPauseMenu instance;

    [SerializeField] string homeScene;

    [SerializeField] GameObject pauseMenu;

    [SerializeField] Animator pauseMenuAnimator;

    bool pauseMenuActive;

    void Awake()
    {
        instance = this;
    }

    public void TogglePauseMenu()
    {
        if (!pauseMenuActive)
        {
            pauseMenuAnimator.Play("PauseMenuDown");
            pauseMenuActive = true;
        }
        else
        {
            pauseMenuAnimator.Play("PauseMenuUp");
            pauseMenuActive = false;
        }
    }

    public void GoToHomeMenu()
    {
        SceneManager.LoadScene(homeScene);
    }
}
