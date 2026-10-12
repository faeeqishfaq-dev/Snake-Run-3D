using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AdController : MonoBehaviour
{
    [Header("Ad Settings")]
    public float adInterval = 10f;

    [Header("UI References")]
    public GameObject adCountdownPanel;
    public Text countdownText;
    
    private float adTimer = 0f;
    private bool isCountingDown = false;
    private bool isAdShowing = false;

    void Start()
    {
        if (adCountdownPanel != null)
        {
            adCountdownPanel.SetActive(false);
        }

        if (PlayerPrefs.GetInt("RemoveAds", 0) == 1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            ResetTimer();
        }
    }
    
    void OnEnable()
    {
        ResetTimer();
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("RemoveAds", 0) == 1) return;
        if (isAdShowing) return;

        if (Time.timeScale > 0)
        {
            adTimer += Time.deltaTime;

            // Trigger countdown in the last 3 seconds of the interval
            if (!isCountingDown && adTimer >= adInterval - 3f && adCountdownPanel != null)
            {
                StartCoroutine(ShowCountdownCoroutine());
            }

            if (adTimer >= adInterval)
            {
                adTimer = 0f;
                ShowAdPanel();
            }
        }
    }

    IEnumerator ShowCountdownCoroutine()
    {
        isCountingDown = true;
        if (adCountdownPanel != null)
        {
            adCountdownPanel.SetActive(true);
            int countdownTime = 3;
            while (countdownTime > 0 && isCountingDown)
            {
                if (countdownText != null)
                {
                    countdownText.text = $"00:{countdownTime:00}";
                }
                yield return new WaitForSecondsRealtime(1f);
                countdownTime--;
            }
            if (adCountdownPanel != null) adCountdownPanel.SetActive(false);
        }
    }

    void ShowAdPanel()
    {
        if (isAdShowing) return;

        isAdShowing = true;
        isCountingDown = false;

        if (adCountdownPanel != null)
            adCountdownPanel.SetActive(false);

        if (CheckCpAd.instance != null)
            CheckCpAd.instance.CallAd();
    }
    public void OnAdClosed()
    {
        isAdShowing = false;
        Time.timeScale = 1f;
        ResetTimer();
    }

    public void ShowAd()
    {
        if (CheckCpAd.instance != null)
        {
            CheckCpAd.instance.CallAd();
        }
        Time.timeScale = 1f;
        Invoke(nameof(HideAdPanel), 0.5f);
        ResetTimer();
    }

    public void RemoveAdsHandler()
    {
        PlayerPrefs.SetInt("RemoveAds", 1);
        Time.timeScale = 1f;
        Invoke(nameof(HideAdPanel), 0.5f);
        Invoke(nameof(DisableGameObject), 0.5f);
    }

    public void HideAdPanel()
    {
        isAdShowing = false;
    }
    
    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
    
    private void ResetTimer()
    {
        adTimer = 0f;
        isCountingDown = false;
        isAdShowing = false;
    }
}
