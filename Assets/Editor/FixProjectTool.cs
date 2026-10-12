using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using TMPro;

public class FixProjectTool
{
    [MenuItem("SnakeRun/Fix Project Details")]
    public static void FixProject()
    {
        Debug.Log("Fixing Project Tags, Prefabs and Hurdles...");
        
        CreateTags();
        FixExistingPrefabs();
        CreateNewHurdles();
        CreateMultiplierGates();
        UpdateLevelManagerInScene();
        
        Debug.Log("Project Fix Complete!");
    }

    private static void CreateTags()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        
        string[] requiredTags = { "Collectible", "Obstacle", "Saw", "Rival", "Multiplier", "Finish" };
        
        foreach (string tag in requiredTags)
        {
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue.Equals(tag))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
            }
        }
        tagManager.ApplyModifiedProperties();
    }

    private static void FixExistingPrefabs()
    {
        // Fix Orb
        GameObject orb = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Orb.prefab");
        if (orb != null)
        {
            orb.tag = "Collectible";
            Collider col = orb.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
            PrefabUtility.SavePrefabAsset(orb);
        }

        // Fix Obstacle Wall
        GameObject wall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ObstacleWall.prefab");
        if (wall != null)
        {
            wall.tag = "Obstacle";
            Collider col = wall.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
            PrefabUtility.SavePrefabAsset(wall);
        }

        // Fix Saw
        GameObject saw = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Saw.prefab");
        if (saw != null)
        {
            saw.tag = "Saw";
            Collider col = saw.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
            
            SawObstacle so = saw.GetComponent<SawObstacle>();
            if (so != null)
            {
                so.moveHorizontally = true;
                so.moveSpeed = 2f;
                so.moveDistance = 3f;
            }
            PrefabUtility.SavePrefabAsset(saw);
        }

        // Fix Finish Line
        GameObject finish = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FinishLine.prefab");
        if (finish != null)
        {
            finish.tag = "Finish";
            Collider col = finish.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
            PrefabUtility.SavePrefabAsset(finish);
        }
    }

    private static void CreateNewHurdles()
    {
        Material obsMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Obstacle.mat");
        Material sawMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Saw.mat");

        // Moving Block
        if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MovingBlock.prefab") == null)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = "MovingBlock";
            block.transform.localScale = new Vector3(3f, 1f, 1f);
            if(obsMat) block.GetComponent<Renderer>().sharedMaterial = obsMat;
            block.tag = "Obstacle";
            block.GetComponent<Collider>().isTrigger = true;
            MovingObstacle mo = block.AddComponent<MovingObstacle>();
            mo.type = MovingObstacle.MovementType.SideToSide;
            mo.moveSpeed = 3f;
            mo.moveDistance = 2.5f;
            PrefabUtility.SaveAsPrefabAsset(block, "Assets/Prefabs/MovingBlock.prefab");
            Object.DestroyImmediate(block);
        }

        // Sliding Barrier
        if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SlidingBarrier.prefab") == null)
        {
            GameObject barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrier.name = "SlidingBarrier";
            barrier.transform.localScale = new Vector3(4f, 1f, 0.5f);
            if(obsMat) barrier.GetComponent<Renderer>().sharedMaterial = obsMat;
            barrier.tag = "Obstacle";
            barrier.GetComponent<Collider>().isTrigger = true;
            MovingObstacle mo = barrier.AddComponent<MovingObstacle>();
            mo.type = MovingObstacle.MovementType.SideToSide;
            mo.moveSpeed = 2f;
            mo.moveDistance = 4f;
            PrefabUtility.SaveAsPrefabAsset(barrier, "Assets/Prefabs/SlidingBarrier.prefab");
            Object.DestroyImmediate(barrier);
        }

        // Rotating Bar
        if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RotatingBar.prefab") == null)
        {
            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bar.name = "RotatingBar";
            bar.transform.localScale = new Vector3(6f, 0.5f, 0.5f);
            if(obsMat) bar.GetComponent<Renderer>().sharedMaterial = obsMat;
            bar.tag = "Obstacle";
            bar.GetComponent<Collider>().isTrigger = true;
            MovingObstacle mo = bar.AddComponent<MovingObstacle>();
            mo.type = MovingObstacle.MovementType.Rotating;
            mo.rotationSpeed = 150f;
            PrefabUtility.SaveAsPrefabAsset(bar, "Assets/Prefabs/RotatingBar.prefab");
            Object.DestroyImmediate(bar);
        }

        // Double Saw
        if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DoubleSaw.prefab") == null)
        {
            GameObject root = new GameObject("DoubleSaw");
            
            GameObject saw1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            saw1.transform.SetParent(root.transform);
            saw1.transform.localPosition = new Vector3(-2, 0, 0);
            saw1.transform.localRotation = Quaternion.Euler(90, 0, 0);
            saw1.transform.localScale = new Vector3(2f, 0.1f, 2f);
            if(sawMat) saw1.GetComponent<Renderer>().sharedMaterial = sawMat;
            saw1.tag = "Saw";
            saw1.GetComponent<Collider>().isTrigger = true;
            SawObstacle so1 = saw1.AddComponent<SawObstacle>();
            so1.moveHorizontally = true;
            so1.moveDistance = 1.5f;
            so1.moveSpeed = 3f;
            
            GameObject saw2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            saw2.transform.SetParent(root.transform);
            saw2.transform.localPosition = new Vector3(2, 0, 0);
            saw2.transform.localRotation = Quaternion.Euler(90, 0, 0);
            saw2.transform.localScale = new Vector3(2f, 0.1f, 2f);
            if(sawMat) saw2.GetComponent<Renderer>().sharedMaterial = sawMat;
            saw2.tag = "Saw";
            saw2.GetComponent<Collider>().isTrigger = true;
            SawObstacle so2 = saw2.AddComponent<SawObstacle>();
            so2.moveHorizontally = true;
            so2.moveDistance = 1.5f;
            so2.moveSpeed = -3f; // Opposite
            so2.rotationSpeed = -300f; // Opposite

            PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/DoubleSaw.prefab");
            Object.DestroyImmediate(root);
        }
    }

    private static void CreateMultiplierGates()
    {
        for (int i = 1; i <= 5; i++)
        {
            string gateName = "GateX" + i;
            if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/" + gateName + ".prefab") == null)
            {
                GameObject gate = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gate.name = gateName;
                gate.transform.localScale = new Vector3(2f, 4f, 1f);
                gate.GetComponent<Collider>().isTrigger = true;
                gate.tag = "Multiplier";
                
                // Set color based on multiplier
                Color c = Color.white;
                if (i == 1) c = new Color(0.2f, 0.6f, 1f); // Blue
                else if (i == 2) c = new Color(0.3f, 0.9f, 0.3f); // Green
                else if (i == 3) c = new Color(1f, 0.9f, 0.1f); // Yellow
                else if (i == 4) c = new Color(1f, 0.5f, 0f); // Orange
                else if (i == 5) c = new Color(1f, 0.2f, 0.2f); // Red
                
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                mat.color = c;
                AssetDatabase.CreateAsset(mat, "Assets/Materials/Mat_" + gateName + ".mat");
                gate.GetComponent<Renderer>().sharedMaterial = mat;

                GameObject txtObj = new GameObject("Text");
                txtObj.transform.SetParent(gate.transform);
                txtObj.transform.localPosition = new Vector3(0, 0, -0.51f);
                txtObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
                txtObj.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                
                Canvas canvas = txtObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                txtObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 50);
                
                TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
                tmp.text = "X" + i;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.fontSize = 40;
                tmp.color = Color.white;
                tmp.fontStyle = FontStyles.Bold;
                tmp.outlineWidth = 0.2f;
                tmp.outlineColor = new Color32(0, 0, 0, 255);

                MultiplierGate mg = gate.AddComponent<MultiplierGate>();
                mg.multiplier = i;
                mg.multiplierText = tmp;

                PrefabUtility.SaveAsPrefabAsset(gate, "Assets/Prefabs/" + gateName + ".prefab");
                Object.DestroyImmediate(gate);
            }
        }
    }

    private static void UpdateLevelManagerInScene()
    {
        LevelManager lm = Object.FindObjectOfType<LevelManager>();
        if (lm != null)
        {
            lm.trackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Track.prefab");
            lm.orbPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Orb.prefab");
            lm.sawPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Saw.prefab");
            lm.obstacleWallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ObstacleWall.prefab");
            lm.finishPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FinishLine.prefab");

            lm.movingBlockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MovingBlock.prefab");
            lm.slidingBarrierPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SlidingBarrier.prefab");
            lm.rotatingBarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RotatingBar.prefab");
            lm.doubleSawPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DoubleSaw.prefab");
            
            lm.gatePrefabX1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GateX1.prefab");
            lm.gatePrefabX2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GateX2.prefab");
            lm.gatePrefabX3 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GateX3.prefab");
            lm.gatePrefabX4 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GateX4.prefab");
            lm.gatePrefabX5 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GateX5.prefab");
            
            EditorUtility.SetDirty(lm);
            if (!Application.isPlaying)
            {
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            }
        }
    }
}
