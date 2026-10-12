using UnityEngine;
using TMPro;

public class MultiplierGate : MonoBehaviour
{
    public int multiplier = 2;
    public TextMeshProUGUI multiplierText;
    [HideInInspector] public bool collected = false;

    private void Start()
    {
        if (multiplierText != null)
        {
            multiplierText.text = "X" + multiplier;
        }
    }
}
