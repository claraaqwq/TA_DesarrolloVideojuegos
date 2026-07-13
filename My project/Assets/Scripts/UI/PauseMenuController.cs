using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused;

    private void Awake()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}
