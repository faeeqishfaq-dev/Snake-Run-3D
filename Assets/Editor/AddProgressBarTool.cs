using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class AddProgressBarTool
{
    [MenuItem("SnakeRun/Add Progress Bar")]
    public static void AddProgressBar()
    {
        Debug.Log("Adding Progress Bar...");

        UIManager uiManager = Object.FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("No UIManager found in scene!");
            return;
        }

        if (uiManager.progressBarContainer != null)
        {
            Debug.Log("Progress bar already exists.");
            return;
        }

        Transform gameUIPanel = uiManager.gameUIPanel.transform;

        Sprite bgSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        Sprite knobSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        // Container
        GameObject container = new GameObject("ProgressBarContainer");
        container.transform.SetParent(gameUIPanel, false);
        RectTransform containerRT = container.AddComponent<RectTransform>();
        containerRT.anchorMin = new Vector2(0.5f, 1f);
        containerRT.anchorMax = new Vector2(0.5f, 1f);
        containerRT.pivot = new Vector2(0.5f, 1f);
        containerRT.anchoredPosition = new Vector2(0, -250); // Below Level Text (which is at -100 typically, wait GameUI is anchored where?)
        // Let's set it safely
        containerRT.sizeDelta = new Vector2(600, 40);

        // Track Background
        GameObject track = new GameObject("TrackBg");
        track.transform.SetParent(container.transform, false);
        RectTransform trackRT = track.AddComponent<RectTransform>();
        trackRT.anchorMin = new Vector2(0, 0);
        trackRT.anchorMax = new Vector2(1, 1);
        trackRT.sizeDelta = Vector2.zero;
        Image trackImg = track.AddComponent<Image>();
        trackImg.sprite = bgSprite;
        trackImg.type = Image.Type.Sliced;
        trackImg.color = new Color(0.1f, 0.1f, 0.2f, 0.8f);

        // Fill Area
        GameObject fillArea = new GameObject("FillArea");
        fillArea.transform.SetParent(container.transform, false);
        RectTransform fillAreaRT = fillArea.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = new Vector2(0, 0);
        fillAreaRT.anchorMax = new Vector2(1, 1);
        fillAreaRT.sizeDelta = new Vector2(-20, -10); // padding

        // Fill Snake Body
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRT = fill.AddComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0, 0);
        fillRT.anchorMax = new Vector2(1, 1);
        fillRT.sizeDelta = Vector2.zero;
        Image fillImg = fill.AddComponent<Image>();
        fillImg.sprite = bgSprite;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImg.color = new Color(0.2f, 0.6f, 1f); // Snake Blue

        // Snake Head
        GameObject head = new GameObject("SnakeHead");
        head.transform.SetParent(fillArea.transform, false);
        RectTransform headRT = head.AddComponent<RectTransform>();
        headRT.anchorMin = new Vector2(0, 0.5f);
        headRT.anchorMax = new Vector2(0, 0.5f);
        headRT.pivot = new Vector2(0.5f, 0.5f);
        headRT.sizeDelta = new Vector2(50, 50); // Round head
        headRT.anchoredPosition = Vector2.zero;
        Image headImg = head.AddComponent<Image>();
        headImg.sprite = knobSprite;
        headImg.color = new Color(0.1f, 0.5f, 0.9f); // Slightly darker blue

        // Finish Flag (Checkered Pattern manually built to ensure compatibility)
        GameObject flag = new GameObject("FinishFlag");
        flag.transform.SetParent(container.transform, false);
        RectTransform flagRT = flag.AddComponent<RectTransform>();
        flagRT.anchorMin = new Vector2(1, 0.5f);
        flagRT.anchorMax = new Vector2(1, 0.5f);
        flagRT.pivot = new Vector2(0.5f, 0.5f);
        flagRT.sizeDelta = new Vector2(40, 40);
        flagRT.anchoredPosition = new Vector2(20, 0);

        // 4 small squares
        for (int i = 0; i < 4; i++)
        {
            GameObject sq = new GameObject("sq_" + i);
            sq.transform.SetParent(flag.transform, false);
            RectTransform sqRT = sq.AddComponent<RectTransform>();
            sqRT.sizeDelta = new Vector2(20, 20);
            
            float x = (i % 2 == 0) ? -10 : 10;
            float y = (i < 2) ? 10 : -10;
            sqRT.anchoredPosition = new Vector2(x, y);

            Image sqImg = sq.AddComponent<Image>();
            sqImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            sqImg.color = ((i == 0 || i == 3) ? Color.black : Color.white);
        }

        // Assign to UIManager
        uiManager.progressBarContainer = container;
        uiManager.progressFill = fillImg;
        uiManager.progressHead = headRT;

        EditorUtility.SetDirty(uiManager);
        if (!Application.isPlaying)
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        Debug.Log("Progress Bar successfully created and wired!");
    }
}
