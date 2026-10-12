using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class GameBuilder : EditorWindow
{
    [MenuItem("SnakeRun/Build Entire Game")]
    public static void BuildGame()
    {
        Debug.Log("Starting Game Build Process...");

        CreateFolders();
        CreateMaterials();
        CreatePrefabs();
        CreateMainScene();

        Debug.Log("Game Build Process Complete!");
    }

    private static void CreateFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
        if (!AssetDatabase.IsValidFolder("Assets/Textures")) AssetDatabase.CreateFolder("Assets", "Textures");
    }

    private static void CreateMaterials()
    {
        CreateColorMaterial("Mat_SnakeBlue", new Color(0.2f, 0.6f, 1f), 0.8f);
        CreateColorMaterial("Mat_SnakeGreen", new Color(0.3f, 0.9f, 0.3f), 0.8f);
        CreateColorMaterial("Mat_SnakePink", new Color(1f, 0.4f, 0.7f), 0.8f);
        CreateColorMaterial("Mat_Track", new Color(0.95f, 0.95f, 0.95f), 0.2f);
        CreateColorMaterial("Mat_Water", new Color(0.1f, 0.4f, 0.9f), 0.9f);
        CreateColorMaterial("Mat_Obstacle", new Color(0.4f, 0.4f, 0.4f), 0.5f);
        CreateColorMaterial("Mat_Saw", new Color(0.8f, 0.8f, 0.8f), 0.8f);
        CreateColorMaterial("Mat_Collectible", new Color(0.2f, 0.7f, 1f), 0.5f);

        if (AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Finish.mat") == null)
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.SetPixel(0, 0, Color.black); tex.SetPixel(1, 0, Color.white);
            tex.SetPixel(0, 1, Color.white); tex.SetPixel(1, 1, Color.black);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/Textures/Checkerboard.png", bytes);
            AssetDatabase.Refresh();

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material mat = new Material(shader);
            mat.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Checkerboard.png");
            AssetDatabase.CreateAsset(mat, "Assets/Materials/Mat_Finish.mat");
        }
        
        CreateColorMaterial("Mat_Mult2", new Color(1f, 0.3f, 0.3f), 0.5f);
        CreateColorMaterial("Mat_Mult3", new Color(1f, 0.9f, 0.1f), 0.5f);
    }

    private static void CreateColorMaterial(string name, Color color, float smoothness)
    {
        string path = "Assets/Materials/" + name + ".mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(path) == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material mat = new Material(shader);
            mat.color = color;
            mat.SetFloat("_Smoothness", smoothness);
            AssetDatabase.CreateAsset(mat, path);
        }
    }

    private static void CreatePrefabs()
    {
        // Track
        if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Track.prefab") == null)
        {
            GameObject track = GameObject.CreatePrimitive(PrimitiveType.Cube);
            track.name = "Track";
            track.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Track.mat");
            PrefabUtility.SaveAsPrefabAsset(track, "Assets/Prefabs/Track.prefab");
            DestroyImmediate(track);
        }

        // Orb
        if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Orb.prefab") == null)
        {
            GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = "Orb";
            orb.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            orb.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Collectible.mat");
            orb.GetComponent<Collider>().isTrigger = true;
            orb.tag = "Collectible";
            PrefabUtility.SaveAsPrefabAsset(orb, "Assets/Prefabs/Orb.prefab");
            DestroyImmediate(orb);
        }

        // Snake Segment
        if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SnakeSegment.prefab") == null)
        {
            GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            seg.name = "SnakeSegment";
            seg.GetComponent<Collider>().enabled = false;
            PrefabUtility.SaveAsPrefabAsset(seg, "Assets/Prefabs/SnakeSegment.prefab");
            DestroyImmediate(seg);
        }

        // Obstacle Wall
        if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ObstacleWall.prefab") == null)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "ObstacleWall";
            wall.transform.localScale = new Vector3(3f, 1f, 1f);
            wall.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Obstacle.mat");
            wall.tag = "Obstacle";
            PrefabUtility.SaveAsPrefabAsset(wall, "Assets/Prefabs/ObstacleWall.prefab");
            DestroyImmediate(wall);
        }

        // Saw
        if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Saw.prefab") == null)
        {
            GameObject saw = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            saw.name = "Saw";
            saw.transform.localScale = new Vector3(2f, 0.1f, 2f);
            saw.transform.rotation = Quaternion.Euler(90, 0, 0);
            saw.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Saw.mat");
            saw.tag = "Saw";
            saw.AddComponent<SawObstacle>();
            PrefabUtility.SaveAsPrefabAsset(saw, "Assets/Prefabs/Saw.prefab");
            DestroyImmediate(saw);
        }

        // Finish Line
        if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FinishLine.prefab") == null)
        {
            GameObject finish = GameObject.CreatePrimitive(PrimitiveType.Cube);
            finish.name = "FinishLine";
            finish.transform.localScale = new Vector3(8, 0.1f, 4);
            finish.GetComponent<Collider>().isTrigger = true;
            finish.tag = "Finish";
            finish.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Finish.mat");
            PrefabUtility.SaveAsPrefabAsset(finish, "Assets/Prefabs/FinishLine.prefab");
            DestroyImmediate(finish);
        }
        
        // Multiplier Gate
        if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MultiplierGate.prefab") == null)
        {
            GameObject gate = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gate.name = "MultiplierGate";
            gate.transform.localScale = new Vector3(1.5f, 2f, 1.5f);
            gate.GetComponent<Collider>().isTrigger = true;
            gate.tag = "Multiplier";
            gate.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Mult2.mat");
            
            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(gate.transform);
            txtObj.transform.localPosition = new Vector3(0, 1.2f, -0.6f);
            txtObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            txtObj.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            
            Canvas canvas = txtObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            txtObj.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 20);
            
            TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "X2";
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 20;
            tmp.color = Color.white;
            tmp.fontStyle = FontStyles.Bold;
            tmp.outlineWidth = 0.2f;
            tmp.outlineColor = new Color32(0, 0, 0, 255);

            MultiplierGate mg = gate.AddComponent<MultiplierGate>();
            mg.multiplier = 2;
            mg.multiplierText = tmp;

            PrefabUtility.SaveAsPrefabAsset(gate, "Assets/Prefabs/MultiplierGate.prefab");
            DestroyImmediate(gate);
        }
    }

    private static void CreateMainScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        scene.name = "MainScene";

        // Setup Light
        Light dirLight = Object.FindObjectOfType<Light>();
        if (dirLight != null)
        {
            dirLight.color = new Color(1f, 0.95f, 0.9f);
            dirLight.intensity = 1.2f;
            dirLight.transform.rotation = Quaternion.Euler(50, -30, 0);
        }

        // Setup Camera
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.4f, 0.7f, 1f);
            cam.gameObject.AddComponent<CameraController>();
        }

        // Setup GameManager
        GameObject gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();
        
        // Setup LevelManager
        LevelManager lm = gmObj.AddComponent<LevelManager>();
        lm.trackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Track.prefab");
        lm.orbPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Orb.prefab");
        lm.sawPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Saw.prefab");
        lm.obstacleWallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ObstacleWall.prefab");
        lm.finishPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FinishLine.prefab");
        lm.matGreen = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_SnakeGreen.mat");
        lm.matPink = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_SnakePink.mat");
        lm.segmentPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SnakeSegment.prefab");

        // Build UI
        BuildUI();

        // Water Background
        GameObject water = GameObject.CreatePrimitive(PrimitiveType.Plane);
        water.name = "Water";
        water.transform.position = new Vector3(0, -5, 100);
        water.transform.localScale = new Vector3(100, 1, 100);
        water.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Water.mat");

        // Player Snake
        GameObject player = CreateSnake("PlayerSnake", new Vector3(0, 0.5f, 0), "Assets/Materials/Mat_SnakeBlue.mat", true, 7);
        if (cam != null) cam.GetComponent<CameraController>().target = player.transform;

        if (!Application.isPlaying)
        {
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainScene.unity");
        }
        
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainScene.unity", true)
        };
    }

    private static GameObject CreateSnake(string name, Vector3 pos, string matPath, bool isPlayer, int power)
    {
        GameObject snake = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        snake.name = name;
        snake.transform.position = pos;
        snake.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        
        Rigidbody rb = snake.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        if (isPlayer)
        {
            snake.tag = "Player";
            snake.AddComponent<SnakeController>();
        }
        else
        {
            snake.tag = "Rival";
            snake.GetComponent<Collider>().isTrigger = true;
            snake.AddComponent<SnakeAI>();
        }
        
        SnakeBody sb = snake.AddComponent<SnakeBody>();
        sb.segmentPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SnakeSegment.prefab");
        
        SnakeStats stats = snake.AddComponent<SnakeStats>();
        stats.isPlayer = isPlayer;
        stats.power = power;

        // Floating Text Canvas
        GameObject canvasObj = new GameObject("FloatingCanvas");
        canvasObj.transform.SetParent(snake.transform);
        canvasObj.transform.localPosition = new Vector3(0, 2, 0);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);
        
        TextMeshProUGUI tmp = CreateTextUI(canvasObj.transform, "PowerText", power.ToString(), 12, Vector2.zero).GetComponent<TextMeshProUGUI>();
        tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        stats.powerText = tmp;

        return snake;
    }

    private static void BuildUI()
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        canvasObj.AddComponent<GraphicRaycaster>();
        
        GameObject uiManagerObj = new GameObject("UIManager");
        UIManager uiManager = uiManagerObj.AddComponent<UIManager>();

        GameObject mainMenuPanel = CreateUIPanel(canvasObj.transform, "MainMenuPanel", new Color(0,0,0,0.4f));
        CreateTextUI(mainMenuPanel.transform, "Title", "Snake Run", 120, new Vector2(0, 400));
        CreateTextUI(mainMenuPanel.transform, "TapToPlay", "Tap to Play", 70, new Vector2(0, -300));
        uiManager.mainMenuPanel = mainMenuPanel;
        uiManager.levelTextMainMenu = CreateTextUI(mainMenuPanel.transform, "LevelText", "Level 1", 60, new Vector2(0, 600)).GetComponent<TextMeshProUGUI>();

        GameObject gameUIPanel = CreateUIPanel(canvasObj.transform, "GameUIPanel", new Color(0,0,0,0));
        uiManager.gameUIPanel = gameUIPanel;
        uiManager.levelTextGame = CreateTextUI(gameUIPanel.transform, "LevelText", "Level 1", 60, new Vector2(0, 800)).GetComponent<TextMeshProUGUI>();
        
        GameObject completePanel = CreateUIPanel(canvasObj.transform, "LevelCompletePanel", new Color(0,0,0,0.8f));
        uiManager.levelCompletePanel = completePanel;
        uiManager.levelTextComplete = CreateTextUI(completePanel.transform, "LevelText", "Level Complete!", 100, new Vector2(0, 300)).GetComponent<TextMeshProUGUI>();
        CreateButtonUI(completePanel.transform, "NextButton", "Next Level", new Vector2(0, -200), () => GameManager.Instance.NextLevel());
        
        GameObject failedPanel = CreateUIPanel(canvasObj.transform, "LevelFailedPanel", new Color(0.6f,0,0,0.8f));
        uiManager.levelFailedPanel = failedPanel;
        CreateTextUI(failedPanel.transform, "FailedText", "Level Failed!", 100, new Vector2(0, 300));
        CreateButtonUI(failedPanel.transform, "RestartButton", "Restart", new Vector2(0, -200), () => GameManager.Instance.RestartLevel());

        uiManager.ShowMainMenu();
    }

    private static GameObject CreateUIPanel(Transform parent, string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        Image img = panel.AddComponent<Image>();
        img.color = color;
        return panel;
    }

    private static GameObject CreateTextUI(Transform parent, string name, string text, int fontSize, Vector2 anchoredPosition)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        tmp.outlineWidth = 0.2f;
        tmp.outlineColor = new Color32(0, 0, 0, 255);

        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPosition;
        rt.sizeDelta = new Vector2(800, 200);
        return textObj;
    }

    private static void CreateButtonUI(Transform parent, string name, string text, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(1f, 0.4f, 0.7f);
        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(onClick);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPosition;
        rt.sizeDelta = new Vector2(400, 150);

        GameObject txtObj = CreateTextUI(btnObj.transform, "Text", text, 60, Vector2.zero);
        txtObj.GetComponent<TextMeshProUGUI>().color = Color.white;
    }
}
