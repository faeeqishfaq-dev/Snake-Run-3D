using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject mainMenuPanel;
    public GameObject gameUIPanel;
    public GameObject levelCompletePanel;
    public GameObject levelFailedPanel;
    public TextMeshProUGUI levelTextMainMenu;
    public TextMeshProUGUI levelTextGame;
    public TextMeshProUGUI levelTextComplete;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel", 1);
        if(levelTextMainMenu != null) levelTextMainMenu.text = "Level " + level;
        if(levelTextGame != null) levelTextGame.text = "Level " + level;
        if(levelTextComplete != null) levelTextComplete.text = "Level " + level + " Complete!";

        // Dynamically find and bind buttons in the scene (so we don't have to rebuild the scene)
        Button[] buttons = Object.FindObjectsOfType<Button>(true);
        foreach (Button b in buttons)
        {
            if (b.name == "NextButton")
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => { if (GameManager.Instance != null) GameManager.Instance.NextLevel(); });
            }
            else if (b.name == "RestartButton")
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => { if (GameManager.Instance != null) GameManager.Instance.RestartLevel(); });
            }
        }

        // Ensure EventSystem exists for UI interaction
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        if (GameManager.autoStartNextTime)
        {
            GameManager.autoStartNextTime = false;
            if (GameManager.Instance != null) GameManager.Instance.StartGame();
        }
        else
        {
            ShowMainMenu();
        }
    }

    public void ShowMainMenu()
    {
        if(mainMenuPanel) mainMenuPanel.SetActive(true);
        if(gameUIPanel) gameUIPanel.SetActive(false);
        if(levelCompletePanel) levelCompletePanel.SetActive(false);
        if(levelFailedPanel) levelFailedPanel.SetActive(false);
    }

    public void ShowGameUI()
    {
        if(mainMenuPanel) mainMenuPanel.SetActive(false);
        if(gameUIPanel) gameUIPanel.SetActive(true);
    }

    public void ShowLevelComplete()
    {
        CheckCpAd.instance.CallAd();
        if (gameUIPanel) gameUIPanel.SetActive(false);
        if(levelCompletePanel) levelCompletePanel.SetActive(true);
    }

    public void ShowLevelFailed()
    {
        CheckCpAd.instance.CallAd();
        if (gameUIPanel) gameUIPanel.SetActive(false);
        if(levelFailedPanel) levelFailedPanel.SetActive(true);
    }

    [Header("Progress Bar")]
    public GameObject progressBarContainer;
    public Image progressFill;
    public RectTransform progressHead;
    
    private Transform playerTransform;
    private float startZ;
    private float finishZ;

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            if (progressBarContainer != null && !progressBarContainer.activeSelf)
                progressBarContainer.SetActive(true);

            if (playerTransform == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null)
                {
                    playerTransform = p.transform;
                    startZ = playerTransform.position.z;
                }
            }

            if (finishZ <= 0) // Assume finish is always ahead
            {
                GameObject f = GameObject.FindGameObjectWithTag("Finish");
                if (f != null) finishZ = f.transform.position.z;
            }

            if (playerTransform != null && finishZ > startZ)
            {
                float progress = Mathf.Clamp01((playerTransform.position.z - startZ) / (finishZ - startZ));
                
                if (progressFill != null) 
                    progressFill.fillAmount = progress;
                
                if (progressHead != null && progressFill != null)
                {
                    float totalWidth = progressFill.rectTransform.rect.width;
                    progressHead.anchoredPosition = new Vector2(totalWidth * progress, progressHead.anchoredPosition.y);
                }
            }
        }
        else
        {
            if (progressBarContainer != null && progressBarContainer.activeSelf)
                progressBarContainer.SetActive(false);
        }
    }
}
