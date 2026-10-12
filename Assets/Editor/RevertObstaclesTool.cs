using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RevertObstaclesTool
{
    [MenuItem("SnakeRun/Revert Obstacles")]
    public static void RevertObstacles()
    {
        Debug.Log("Reverting Obstacle Prefabs to original primitive state...");

        // Revert Saw
        GameObject saw = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Saw.prefab");
        if (saw != null)
        {
            RevertVisuals(saw);
            PrefabUtility.SavePrefabAsset(saw);
        }

        // Revert Double Saw
        GameObject doubleSaw = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DoubleSaw.prefab");
        if (doubleSaw != null)
        {
            foreach (Transform child in doubleSaw.transform)
            {
                if (child.name.Contains("Saw") || child.GetComponent<Collider>() != null)
                {
                    RevertVisuals(child.gameObject);
                }
            }
            PrefabUtility.SavePrefabAsset(doubleSaw);
        }

        // Revert SlidingBarrier
        GameObject slidingBarrier = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SlidingBarrier.prefab");
        if (slidingBarrier != null)
        {
            RevertVisuals(slidingBarrier);
            PrefabUtility.SavePrefabAsset(slidingBarrier);
        }

        // Revert MovingBlock
        GameObject movingBlock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MovingBlock.prefab");
        if (movingBlock != null)
        {
            RevertVisuals(movingBlock);
            PrefabUtility.SavePrefabAsset(movingBlock);
        }

        // Revert ObstacleWall
        GameObject obstacleWall = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ObstacleWall.prefab");
        if (obstacleWall != null)
        {
            RevertVisuals(obstacleWall);
            PrefabUtility.SavePrefabAsset(obstacleWall);
        }

        Debug.Log("Obstacles reverted successfully!");
    }

    private static void RevertVisuals(GameObject parent)
    {
        // Re-enable root renderer
        MeshRenderer mr = parent.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = true;

        // Destroy generated visuals
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform t in parent.transform)
        {
            if (t.name.StartsWith("Visual_")) toDestroy.Add(t.gameObject);
        }
        foreach (GameObject g in toDestroy) GameObject.DestroyImmediate(g, true);
    }
}
