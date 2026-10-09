
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    public bool IsPaused { get; private set; }

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        // Press ESC to pause or resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void PauseGame()
    {
        if (IsPaused) return;

        IsPaused = true;
        Time.timeScale = 0f;

        if (pauseMenu != null)
            pauseMenu.SetActive(true);

        Debug.Log("TL5: Game Paused");
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        Debug.Log("TL5: Game Resumed");
    }

    public void TogglePause()
    {
        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void OnDestroy()
    {
        if (IsPaused)
            Time.timeScale = 1f;
    }
}
