using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    public Vector2 PointerPosition { get; set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateWeaponRotation();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWeaponRotation();
    }

    void UpdateWeaponRotation()
    {
        transform.right = (PointerPosition - (Vector2)transform.position).normalized;
    }
}
