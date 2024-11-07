using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextControl : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TextMeshProUGUI textMeshProUGUI;

    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        textMeshProUGUI.text = text;
    }

    public void SetColor(Color color)
    {
        textMeshProUGUI.color = color;
    }
}
