using UnityEngine;

public class RenderLayerController : MonoBehaviour
{
    public string frontRenderLayer = "Default";
    public string backRenderLayer = "Background";

    void Update()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            if (transform.position.y > 0)
            {
                renderer.sortingLayerName = frontRenderLayer;
            }
            else
            {
                renderer.sortingLayerName = backRenderLayer;
            }
        }
    }
}