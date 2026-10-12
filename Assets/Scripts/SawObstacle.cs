using UnityEngine;

public class SawObstacle : MonoBehaviour
{
    public float rotationSpeed = 300f;
    public bool moveHorizontally = true;
    public float moveSpeed = 2f;
    public float moveDistance = 3f;
    
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Continuous rotation around its local up axis (which is usually the flat face of the cylinder)
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
        
        if (moveHorizontally)
        {
            float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            transform.position = startPos + new Vector3(offset, 0, 0);
        }
    }
}
