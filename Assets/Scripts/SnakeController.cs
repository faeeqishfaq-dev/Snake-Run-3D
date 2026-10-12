using UnityEngine;
using UnityEngine.EventSystems;

public class SnakeController : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float steerSpeed = 15f;
    public float roadWidth = 4f;

    private float _currentX;
    private Vector3 _startTouchPos;
    private float _startX;
    
    private void Start()
    {
        _currentX = transform.position.x;
    }

    private void Update()
    {
        if (GameManager.Instance.currentState == GameManager.GameState.MainMenu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.StartGame();
                _startTouchPos = Input.mousePosition;
                _startX = _currentX;
            }
            return;
        }

        if (GameManager.Instance.currentState != GameManager.GameState.Playing) return;

        HandleInput();
        MoveForward();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startTouchPos = Input.mousePosition;
            _startX = _currentX;
        }
        else if (Input.GetMouseButton(0))
        {
            float deltaX = (Input.mousePosition.x - _startTouchPos.x) / (Screen.width * 0.5f);
            _currentX = _startX + deltaX * steerSpeed;
            _currentX = Mathf.Clamp(_currentX, -roadWidth, roadWidth);
        }
    }

    private void MoveForward()
    {
        Vector3 newPos = transform.position;
        newPos.z += forwardSpeed * Time.deltaTime;
        
        float newX = Mathf.Lerp(transform.position.x, _currentX, 15f * Time.deltaTime);
        newPos.x = newX;
        
        Vector3 moveDir = new Vector3(newX - transform.position.x, 0, forwardSpeed * Time.deltaTime);
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 20f * Time.deltaTime);
        }

        transform.position = newPos;
    }
}
