using UnityEngine;
using System.Collections.Generic;

public class SnakeBody : MonoBehaviour
{
    public GameObject segmentPrefab;
    public float segmentDistance = 0.6f;

    private List<Transform> _segments = new List<Transform>();
    private List<Vector3> _positionsHistory = new List<Vector3>();

    private void Awake()
    {
        _segments.Add(transform);
        _positionsHistory.Add(transform.position);
    }

    public void Initialize(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddSegment();
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Playing && GameManager.Instance.currentState != GameManager.GameState.Finished) return;

        if (Vector3.Distance(transform.position, _positionsHistory[0]) > 0.05f)
        {
            _positionsHistory.Insert(0, transform.position);
        }

        int index = 0;
        foreach (Transform segment in _segments)
        {
            if (segment == transform) continue;

            // Distance based index calculation
            int histIndex = index * Mathf.RoundToInt(segmentDistance / 0.05f);
            histIndex = Mathf.Clamp(histIndex, 0, _positionsHistory.Count - 1);
            
            Vector3 point = _positionsHistory[histIndex];
            
            segment.position = Vector3.Lerp(segment.position, point, Time.deltaTime * 20f);
            
            Vector3 lookDir = segment.position - point;
            if(lookDir != Vector3.zero)
            {
                segment.rotation = Quaternion.Lerp(segment.rotation, Quaternion.LookRotation(-lookDir), Time.deltaTime * 20f);
            }
            
            index++;
        }

        if (_positionsHistory.Count > _segments.Count * Mathf.RoundToInt(segmentDistance / 0.05f) + 10)
        {
            _positionsHistory.RemoveAt(_positionsHistory.Count - 1);
        }
    }

    public void AddSegment()
    {
        if (segmentPrefab == null) return;
        Transform lastSegment = _segments[_segments.Count - 1];
        GameObject newSegment = Instantiate(segmentPrefab, lastSegment.position, lastSegment.rotation);
        
        Renderer segmentRenderer = newSegment.GetComponent<Renderer>();
        Renderer headRenderer = GetComponent<Renderer>();
        if (segmentRenderer != null && headRenderer != null)
        {
            segmentRenderer.sharedMaterial = headRenderer.sharedMaterial;
        }
        
        newSegment.transform.parent = null;
        _segments.Add(newSegment.transform);
    }
    
    public void RemoveSegment()
    {
        if (_segments.Count > 1)
        {
            Transform last = _segments[_segments.Count - 1];
            _segments.RemoveAt(_segments.Count - 1);
            Destroy(last.gameObject);
        }
    }
}
