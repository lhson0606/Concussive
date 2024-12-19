using UnityEngine;

public class BombController : MonoBehaviour
{


    Collider2D innerCol;
    Collider2D outerCol;
    private void Awake()
    {
        innerCol = transform.Find("Inner")?.GetComponent<Collider2D>();
        outerCol = transform.Find("Outer")?.GetComponent<Collider2D>();

        if (innerCol == null || outerCol == null)
        {
            Debug.LogError("BombController: Inner or Outer collider is missing");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealExplosionDamage()
    {

    }
}
