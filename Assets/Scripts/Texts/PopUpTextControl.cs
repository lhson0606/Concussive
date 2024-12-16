using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PopUpTextControl : SlowMotionObject
{
    public float lifeTime = 1f;
    public Vector2 InitialVelocity = new Vector2(0, 1);
    public Vector2 InitialVelocityRandomness = new Vector2(-0.5f, 0.5f);
    public Color textColor = Color.white;
    public string text = "Text";
    public Vector2 scale = new Vector2(1, 1);

    private TextMeshProUGUI textMesh;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeComponents();
        SetTextProperties();
        ApplyInitialVelocity();
        ScheduleDestruction();
    }

    private void InitializeComponents()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        rb = GetComponent<Rigidbody2D>();

        if (textMesh == null)
        {
            Debug.LogError("TextMeshProUGUI component not found in children.");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found.");
        }
    }

    private void SetTextProperties()
    {
        if (textMesh != null)
        {
            textMesh.text = text;
            textMesh.color = textColor;
            textMesh.transform.localScale = new Vector3(scale.x, scale.y, 1);
        }
    }

    private void ApplyInitialVelocity()
    {
        if (rb != null)
        {
            rb.linearVelocity = GetRandomVelocity();
        }
    }

    private Vector2 GetRandomVelocity()
    {
        return new Vector2(
            Random.Range(InitialVelocity.x + InitialVelocityRandomness.x, InitialVelocity.x + InitialVelocityRandomness.y),
            Random.Range(InitialVelocity.y + InitialVelocityRandomness.x, InitialVelocity.y + InitialVelocityRandomness.y)
        );
    }

    private void ScheduleDestruction()
    {
        Destroy(gameObject, lifeTime);
    }
}
