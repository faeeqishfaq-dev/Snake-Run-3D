using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public enum MovementType { SideToSide, Rotating, OpeningClosing }
    public MovementType type = MovementType.SideToSide;

    public float moveSpeed = 3f;
    public float moveDistance = 3f;
    public float rotationSpeed = 100f;
    
    private Vector3 startPos;
    private Quaternion startRot;

    private void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    private void Update()
    {
        switch (type)
        {
            case MovementType.SideToSide:
                float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
                transform.position = startPos + new Vector3(offset, 0, 0);
                break;
                
            case MovementType.Rotating:
                transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
                break;
                
            case MovementType.OpeningClosing:
                float scaleOffset = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
                transform.localScale = new Vector3(scaleOffset, transform.localScale.y, transform.localScale.z);
                break;
        }
    }
}
