using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class RevertSnakeUITool
{
    [MenuItem("SnakeRun/Revert Snake UI")]
    public static void RevertSnakeUI()
    {
        Debug.Log("Reverting Snake UI to original state...");

        // 1. Remove bubbleSprite from LevelManager (already removed from script, but just in case it had issues)
        // Since we removed the field from script, it won't serialize it.

        // 2. Revert Player Snake in the scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SnakeStats stats = player.GetComponent<SnakeStats>();
            if (stats != null && stats.powerText != null)
            {
                Transform canvasTransform = stats.powerText.transform.parent;
                
                // Remove BubbleBg
                Transform oldBg = canvasTransform.Find("BubbleBg");
                if (oldBg != null) GameObject.DestroyImmediate(oldBg.gameObject);

                // Revert Text
                TextMeshProUGUI tmp = stats.powerText;
                tmp.color = Color.white;
                tmp.outlineWidth = 0.2f;
                tmp.outlineColor = new Color32(0, 0, 0, 255);
                tmp.fontSize = 12;
                
                RectTransform txtRT = tmp.GetComponent<RectTransform>();
                txtRT.sizeDelta = new Vector2(2, 2);
                txtRT.anchoredPosition = Vector2.zero;

                EditorUtility.SetDirty(player);
            }
        }
        if (!Application.isPlaying)
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
}
