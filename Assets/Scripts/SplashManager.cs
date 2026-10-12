using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SplashManager : MonoBehaviour
{
    public GameObject loadingPanel;
    public GameObject mainMenuPanel;
    public Image loadingFill;
    public TextMeshProUGUI loadingText;
    public Button playButton;
    public RectTransform playButtonRect;
    public RectTransform snakeIconRect;

    [Header("Loading Settings")]
    [SerializeField] private float loadingFillTime = 3f;

    public RectTransform snakeHeadRect;

    private void Start()
    {
        loadingPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayClicked);
        }
        
        StartCoroutine(SimulateFirstLoad());
    }

    private void Update()
    {
        if (mainMenuPanel.activeSelf)
        {
            // Pulse play button for polished casual feel
            if (playButtonRect != null)
            {
                playButtonRect.localScale = Vector3.one * (1f + Mathf.Sin(Time.time * 5f) * 0.04f);
            }
            
            // Float snake icon gently
            if (snakeIconRect != null)
            {
                snakeIconRect.anchoredPosition = new Vector2(0, -80f + Mathf.Sin(Time.time * 3f) * 20f);
            }
        }
    }

    private void UpdateSnakeHead(float progress)
    {
        if (snakeHeadRect == null)
        {
            if (loadingFill != null && loadingFill.transform.parent != null)
            {
                Transform head = loadingFill.transform.parent.Find("SnakeHead");
                if (head != null)
                {
                    snakeHeadRect = head.GetComponent<RectTransform>();
                }
            }
        }

        if (snakeHeadRect != null && loadingFill != null)
        {
            // Force left-anchor so positioning is consistent regardless of how it was created
            snakeHeadRect.anchorMin = new Vector2(0, 0.5f);
            snakeHeadRect.anchorMax = new Vector2(0, 0.5f);
            
            float leftPadding = loadingFill.rectTransform.offsetMin.x;
            float totalWidth = loadingFill.rectTransform.rect.width;
            
            // Move only on X axis. Keep its original Y position.
            snakeHeadRect.anchoredPosition = new Vector2(leftPadding + (totalWidth * progress), snakeHeadRect.anchoredPosition.y);
        }
    }

    private IEnumerator SimulateFirstLoad()
    {
        if (loadingFill != null) loadingFill.fillAmount = 0f;
        if (loadingText != null) loadingText.text = "LOADING 0%";
        UpdateSnakeHead(0f);

        float timer = 0f;
        while (timer < loadingFillTime)
        {
            timer += Time.deltaTime;
            float clampedProgress = Mathf.Clamp01(timer / loadingFillTime);
            
            if (loadingFill != null) loadingFill.fillAmount = clampedProgress;
            if (loadingText != null) loadingText.text = "LOADING " + Mathf.Clamp(Mathf.RoundToInt(clampedProgress * 100f), 0, 100) + "%";
            UpdateSnakeHead(clampedProgress);
            
            yield return null;
        }
        
        if (loadingFill != null) loadingFill.fillAmount = 1f;
        if (loadingText != null) loadingText.text = "LOADING 100%";
        UpdateSnakeHead(1f);
        
        yield return new WaitForSeconds(0.25f);
        
        loadingPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void PlayClicked()
    {
        mainMenuPanel.SetActive(false);
        loadingPanel.SetActive(true);
        
        // Pass autoStartNextTime so it skips the "Tap To Play" screen in MainScene
        GameManager.autoStartNextTime = true;

        CheckCpAd.instance.CallAd();

        StartCoroutine(LoadMainSceneAsync());
    }

    private IEnumerator LoadMainSceneAsync()
    {
        if (loadingFill != null) loadingFill.fillAmount = 0f;
        if (loadingText != null) loadingText.text = "LOADING 0%";
        UpdateSnakeHead(0f);
        //CheckCpAd.instance.CallAd();
        AsyncOperation op = SceneManager.LoadSceneAsync("MainScene");
        op.allowSceneActivation = false;

        float timer = 0f;

        // Loop until both our required visual time has passed AND the scene has fully loaded in the background
        while (timer < loadingFillTime || op.progress < 0.9f)
        {
            timer += Time.deltaTime;
            float visualProgress = Mathf.Clamp01(timer / loadingFillTime);

            // Stall visual progress at 99% if the minimum time has elapsed but the scene is still loading
            if (visualProgress >= 1f && op.progress < 0.9f)
            {
                visualProgress = 0.99f;
            }

            if (loadingFill != null) loadingFill.fillAmount = visualProgress;
            if (loadingText != null) loadingText.text = "LOADING " + Mathf.Clamp(Mathf.RoundToInt(visualProgress * 100f), 0, 100) + "%";
            UpdateSnakeHead(visualProgress);
            
            yield return null;
        }

        if (loadingFill != null) loadingFill.fillAmount = 1f;
        if (loadingText != null) loadingText.text = "LOADING 100%";
        UpdateSnakeHead(1f);
        
        yield return new WaitForSeconds(0.3f); // Small delay so player sees 100%

        op.allowSceneActivation = true;
    }
}
