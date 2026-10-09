using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public GameState gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState.gameTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        gameState.gameTimer += Time.deltaTime;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
