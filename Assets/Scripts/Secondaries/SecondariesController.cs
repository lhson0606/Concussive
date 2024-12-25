using UnityEngine;
using UnityEngine.TextCore.Text;

public class SecondariesController : MonoBehaviour
{
    private Vector2 TargetPosition { get; set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = (TargetPosition - (Vector2)transform.position).normalized;
        transform.right = direction;
    }
}
