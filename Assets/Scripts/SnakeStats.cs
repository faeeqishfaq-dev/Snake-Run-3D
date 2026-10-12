using UnityEngine;
using TMPro;

public class SnakeStats : MonoBehaviour
{
    public int power = 7;
    public TextMeshProUGUI powerText;
    public bool isPlayer = false;
    
    private SnakeBody _body;
    private Collider _collider;

    private void Start()
    {
        _body = GetComponent<SnakeBody>();
        _collider = GetComponent<Collider>();
        
        UpdatePowerText();
        if (_body != null)
        {
            _body.Initialize(Mathf.Clamp(power / 2, 1, 30)); 
        }
    }

    public void AddPower(int amount)
    {
        power += amount;
        UpdatePowerText();
        if (_body != null && amount > 0)
        {
            if (power % 2 == 0) _body.AddSegment();
        }
    }

    public void RemovePower(int amount)
    {
        power -= amount;
        UpdatePowerText();
        if (_body != null && amount > 0)
        {
             if (power % 2 == 0) _body.RemoveSegment();
        }
        if (power <= 0)
        {
            Die();
        }
    }

    private void UpdatePowerText()
    {
        if (powerText != null)
        {
            powerText.text = power.ToString();
        }
    }

    public void Die()
    {
        if (isPlayer)
        {
            GameManager.Instance.LevelFailed();
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        if (!isPlayer) return;

        if (other.CompareTag("Collectible"))
        {
            if (other.activeInHierarchy)
            {
                other.SetActive(false); // Prevents multiple triggers in the same frame
                AddPower(1);
                Destroy(other);
            }
        }
        else if (other.CompareTag("Obstacle") || other.CompareTag("Saw"))
        {
            Die();
        }
        else if (other.CompareTag("Finish"))
        {
            GameManager.Instance.LevelComplete();
            
            SnakeController ctrl = GetComponent<SnakeController>();
            if(ctrl) ctrl.enabled = false;
            StartCoroutine(WalkToChests());
        }
        else if (other.CompareTag("Rival"))
        {
            SnakeStats rival = other.GetComponent<SnakeStats>();
            if (rival != null && rival.gameObject.activeInHierarchy)
            {
                if (this.power > rival.power)
                {
                    this.AddPower(rival.power);
                    rival.Die();
                }
                else
                {
                    Die();
                }
            }
        }
        else if (other.CompareTag("Multiplier"))
        {
            MultiplierGate gate = other.GetComponent<MultiplierGate>();
            if(gate != null && !gate.collected)
            {
                gate.collected = true;
                power *= gate.multiplier;
                UpdatePowerText();
            }
        }
    }

    private System.Collections.IEnumerator WalkToChests()
    {
        while(GameManager.Instance.currentState == GameManager.GameState.Finished)
        {
            transform.position += Vector3.forward * 10f * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0,0), 10f * Time.deltaTime);
            yield return null;
        }
    }
}
