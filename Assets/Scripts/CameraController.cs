using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 8, -10);
    public float smoothSpeed = 10f;
    public float pitch = 25f;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(pitch, 0, 0);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = new Vector3(0, target.position.y + offset.y, target.position.z + offset.z);
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
    }
}
