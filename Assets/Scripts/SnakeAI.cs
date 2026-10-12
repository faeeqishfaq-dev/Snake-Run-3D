using UnityEngine;

public class SnakeAI : MonoBehaviour
{
    public float forwardSpeed = 8f;
    public float sideSpeed = 2f;
    private float targetX;

    private void Start()
    {
        targetX = transform.position.x;
        InvokeRepeating("ChangeLane", 1f, 2f);
    }

    private void ChangeLane()
    {
        targetX = Random.Range(-3f, 3f);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Playing) return;

        Vector3 newPos = transform.position;
        newPos.z += forwardSpeed * Time.deltaTime;
        newPos.x = Mathf.Lerp(transform.position.x, targetX, sideSpeed * Time.deltaTime);
        
        Vector3 moveDir = new Vector3(newPos.x - transform.position.x, 0, forwardSpeed * Time.deltaTime);
        if (moveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), 10f * Time.deltaTime);
        }

        transform.position = newPos;
    }
}
