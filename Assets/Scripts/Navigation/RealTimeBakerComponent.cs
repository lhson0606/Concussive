using UnityEngine;
using NavMeshPlus.Components;

public class RealTimeBakerComponent : MonoBehaviour
{
    private NavMeshSurface surface2D;

    public void Awake()
    {
        surface2D = GetComponent<NavMeshSurface>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        surface2D.BuildNavMeshAsync();
    }

    // Update is called once per frame
    void Update()
    {
        surface2D.UpdateNavMesh(surface2D.navMeshData);
    }
}
