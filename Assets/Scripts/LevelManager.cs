using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [Header("Core Prefabs")]
    public GameObject trackPrefab;
    public GameObject orbPrefab;
    public GameObject finishPrefab;
    
    [Header("Obstacle Prefabs")]
    public GameObject sawPrefab;
    public GameObject doubleSawPrefab;
    public GameObject movingBlockPrefab;
    public GameObject slidingBarrierPrefab;
    public GameObject rotatingBarPrefab;
    public GameObject obstacleWallPrefab;
    
    [Header("Multiplier Prefabs")]
    public GameObject gatePrefabX1;
    public GameObject gatePrefabX2;
    public GameObject gatePrefabX3;
    public GameObject gatePrefabX4;
    public GameObject gatePrefabX5;
    
    [Header("Rival Settings")]
    public Material matGreen;
    public Material matPink;
    public GameObject segmentPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    private void Start()
    {
        GenerateLevel();
    }
    
    public void GenerateLevel()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel", 1);
        int trackLength = 15 + (level * 2); // gets longer
        
        GameObject levelRoot = new GameObject("LevelRoot");

        // Base Track
        GameObject track = Instantiate(trackPrefab, new Vector3(0, -0.5f, trackLength * 10f), Quaternion.identity, levelRoot.transform);
        track.transform.localScale = new Vector3(8f, 1f, trackLength * 20f + 80f); // extend past finish
        
        // Spawn Orbs & Obstacles in alternating patterns
        for (int i = 1; i < trackLength; i++)
        {
            float zPos = i * 20f;
            int patternType = i % 5;
            
            // Spawn Collectibles
            int numOrbs = Random.Range(2, 5);
            for(int o = 0; o < numOrbs; o++)
            {
                Instantiate(orbPrefab, new Vector3(Random.Range(-3f, 3f), 0.5f, zPos - 10f + Random.Range(-2f, 2f)), Quaternion.identity, levelRoot.transform);
            }

            // Spawn Obstacles based on pattern
            if (i > 1)
            {
                Vector3 obsPos = new Vector3(0, 1f, zPos);
                switch(patternType)
                {
                    case 0:
                        // Double Saw
                        if (doubleSawPrefab != null) Instantiate(doubleSawPrefab, obsPos, Quaternion.identity, levelRoot.transform);
                        else if (sawPrefab != null) Instantiate(sawPrefab, obsPos, Quaternion.Euler(0, 0, 0), levelRoot.transform);
                        break;
                    case 1:
                        // Moving Block
                        if (movingBlockPrefab != null) Instantiate(movingBlockPrefab, obsPos, Quaternion.identity, levelRoot.transform);
                        else if (obstacleWallPrefab != null) Instantiate(obstacleWallPrefab, obsPos + new Vector3(Random.Range(-2f,2f), 0, 0), Quaternion.identity, levelRoot.transform);
                        break;
                    case 2:
                        // Sliding Barrier
                        if (slidingBarrierPrefab != null) Instantiate(slidingBarrierPrefab, obsPos + new Vector3(-2f, 0, 0), Quaternion.Euler(0f,0f, 0f), levelRoot.transform);
                        //else if (slidingBarrierPrefab != null) Instantiate(slidingBarrierPrefab, transform.rotation = Quaternion.Euler(-90,);
                        break;
                    case 3:
                        // Rotating Bar
                        if (rotatingBarPrefab != null) Instantiate(rotatingBarPrefab, obsPos, Quaternion.identity, levelRoot.transform);
                        else if (sawPrefab != null) Instantiate(sawPrefab, obsPos, Quaternion.Euler(0, 0, 0), levelRoot.transform);
                        break;
                    case 4:
                        // Moving Saw
                        if (sawPrefab != null) Instantiate(sawPrefab, obsPos, Quaternion.Euler(0, 0, 0), levelRoot.transform);
                        break;
                }
            }
            
            // Spawn Rivals (start appearing after level 2)
            if (level >= 2 && i % 3 == 0 && Random.value > 0.4f)
            {
                CreateRival(new Vector3(Random.Range(-2f, 2f), 1f, zPos + 5), levelRoot.transform, Random.Range(3, 5 + level * 2));
            }
        }
        
        // Finish Line
        float finishZ = trackLength * 20f;
        if(finishPrefab != null) Instantiate(finishPrefab, new Vector3(0, 0.1f, finishZ), Quaternion.identity, levelRoot.transform);

        // Multipliers (Progressive)
        SpawnMultiplier(gatePrefabX1, new Vector3(-2, 1, finishZ + 10), levelRoot.transform);
        SpawnMultiplier(gatePrefabX2, new Vector3(2, 1, finishZ + 10), levelRoot.transform);
        
        SpawnMultiplier(gatePrefabX2, new Vector3(-2, 1, finishZ + 25), levelRoot.transform);
        SpawnMultiplier(gatePrefabX3, new Vector3(2, 1, finishZ + 25), levelRoot.transform);
        
        SpawnMultiplier(gatePrefabX4, new Vector3(-2, 1, finishZ + 40), levelRoot.transform);
        SpawnMultiplier(gatePrefabX5, new Vector3(2, 1, finishZ + 40), levelRoot.transform);
    }
    
    private void SpawnMultiplier(GameObject prefab, Vector3 pos, Transform parent)
    {
        if (prefab != null)
        {
            Instantiate(prefab, pos, Quaternion.identity, parent);
        }
    }
    
    private void CreateRival(Vector3 pos, Transform parent, int power)
    {
        GameObject snake = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        snake.name = "RivalSnake";
        snake.transform.position = pos;
        snake.transform.SetParent(parent);
        
        bool isGreen = Random.value > 0.5f;
        if (snake.GetComponent<Renderer>() != null)
            snake.GetComponent<Renderer>().sharedMaterial = isGreen ? matGreen : matPink;
        
        Rigidbody rb = snake.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        snake.tag = "Rival";
        Collider col = snake.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
        
        SnakeAI ai = snake.AddComponent<SnakeAI>();
        ai.forwardSpeed = Random.Range(6f, 9f);
        
        SnakeBody sb = snake.AddComponent<SnakeBody>();
        sb.segmentPrefab = segmentPrefab;
        
        SnakeStats stats = snake.AddComponent<SnakeStats>();
        stats.isPlayer = false;
        stats.power = power;

        // Clone PlayerSnake UI to ensure perfect visual match
        GameObject player = GameObject.Find("PlayerSnake");
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        
        bool cloned = false;
        if (player != null)
        {
            Canvas playerCanvas = player.GetComponentInChildren<Canvas>();
            if (playerCanvas != null)
            {
                GameObject canvasObj = Instantiate(playerCanvas.gameObject, snake.transform);
                canvasObj.name = "FloatingCanvas";
                canvasObj.transform.localPosition = playerCanvas.transform.localPosition;
                canvasObj.transform.localRotation = playerCanvas.transform.localRotation;
                canvasObj.transform.localScale = playerCanvas.transform.localScale;
                
                TextMeshProUGUI tmp = canvasObj.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text = power.ToString();
                    stats.powerText = tmp;
                }
                cloned = true;
            }
        }

        if (!cloned)
        {
            // Fallback Floating Text Canvas
            GameObject canvasObj = new GameObject("FloatingCanvas");
            canvasObj.transform.SetParent(snake.transform);
            canvasObj.transform.localPosition = new Vector3(0, 2, 0);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasObj.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);
    
            GameObject textObj = new GameObject("PowerText");
            textObj.transform.SetParent(canvasObj.transform, false);
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = power.ToString();
            tmp.fontSize = 12;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.fontStyle = FontStyles.Bold;
            tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);
            
            // Add Outline
            tmp.outlineWidth = 0.2f;
            tmp.outlineColor = new Color32(0, 0, 0, 255);
            
            stats.powerText = tmp;
        }
    }
}
