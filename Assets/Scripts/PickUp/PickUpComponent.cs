using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(GameItem))]
public class PickUpComponent : MonoBehaviour, IPickUpable
{
    private bool isSelected = false;
    private LineRenderer lineRenderer;
    // Define an event to notify when the item is picked up
    public event Action<BaseCharacter> OnPickedUp;
    private bool isPlayerInRange = false;
    private GameItem gameItem;
    private BaseCharacter currentBaseCharacter = null;

    public void OnDeselect()
    {
        isSelected = false;
        lineRenderer.enabled = false;
        gameItem.StopDisplayingItemName();
        this.currentBaseCharacter = null;
    }

    public PickUpComponent()
    {

    }

    public void OnPickUp()
    {
        if(!isPlayerInRange)
        {
            return;
        }

        isSelected = false;
        // Notify the owner that the item has been picked up
        OnPickedUp?.Invoke(currentBaseCharacter);
        OnDeselect();
        Destroy(this);
    }

    public void OnSelect(BaseCharacter target)
    {
        isSelected = true;
        lineRenderer.enabled = true;
        this.currentBaseCharacter = target;
        gameItem.DisplayItemName();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.enabled = false;

        gameItem = GetComponent<GameItem>();

        // Set a material for the LineRenderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(gameItem.GetRarityColor().r, gameItem.GetRarityColor().g, gameItem.GetRarityColor().b, 0.5f);
        lineRenderer.endColor = new Color(gameItem.GetRaritySecondaryColor().r, gameItem.GetRaritySecondaryColor().g, gameItem.GetRaritySecondaryColor().b, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            DrawCollider();
        }
    }

    private void DrawCollider()
    {
        // Draw the collider shape
        Collider2D collider = GetComponent<Collider2D>();
        if (collider is BoxCollider2D boxCollider)
        {
            Vector3[] points = new Vector3[5];
            Vector3 center = boxCollider.bounds.center;
            Vector3 size = boxCollider.bounds.size;
            points[0] = center + new Vector3(-size.x, -size.y) * 0.5f;
            points[1] = center + new Vector3(size.x, -size.y) * 0.5f;
            points[2] = center + new Vector3(size.x, size.y) * 0.5f;
            points[3] = center + new Vector3(-size.x, size.y) * 0.5f;
            points[4] = points[0]; // Close the loop
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
        else if (collider is CircleCollider2D circleCollider)
        {
            int segments = 36;
            Vector3[] points = new Vector3[segments + 1];
            Vector3 center = circleCollider.bounds.center;
            float radius = circleCollider.radius;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2 * Mathf.PI / segments;
                points[i] = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
        else if (collider is PolygonCollider2D polygonCollider)
        {
            Vector2[] points2D = polygonCollider.points;
            Vector3[] points = new Vector3[points2D.Length + 1];
            for (int i = 0; i < points2D.Length; i++)
            {
                points[i] = transform.TransformPoint(points2D[i]);
            }
            points[points2D.Length] = points[0]; // Close the loop
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if this is already selected by the player, return
        if (isSelected)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if(!playerController.HasSelectedPickUp())
            {
                playerController.SetSelectedPickUp(this);
                this.OnSelect(playerController.GetComponent<BaseCharacter>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;

            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (UnityEngine.Object.Equals(playerController.GetSelectedPickUp(), this))
            {
                playerController.SetSelectedPickUp(null);
                this.OnDeselect();
            }
        }
    }
}
