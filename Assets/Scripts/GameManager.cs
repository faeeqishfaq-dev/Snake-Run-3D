using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool autoStartNextTime = false;
    
    public enum GameState { MainMenu, Playing, Finished, Failed }
    public GameState currentState = GameState.MainMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartGame()
    {
        currentState = GameState.Playing;
        UIManager.Instance.ShowGameUI();
    }

    public void LevelComplete()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.Finished;
        StartCoroutine(LevelCompleteRoutine());
    }

    private IEnumerator LevelCompleteRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        UIManager.Instance.ShowLevelComplete();
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);
    }

    public void LevelFailed()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.Failed;
        UIManager.Instance.ShowLevelFailed();
    }

    public void RestartLevel()
    {
        autoStartNextTime = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
