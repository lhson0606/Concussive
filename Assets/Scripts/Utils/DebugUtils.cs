using UnityEngine;

public class DebugUtils
{
    // draw a circle around z axis
    public static void DrawDebugCircle(Vector3 center, float radius, Color color, float duration = 0.1f)
    {
        int segments = 36;
        float angle = 0f;
        float angleStep = 360f / segments;

        Vector3 prevPoint = center + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, Mathf.Sin(Mathf.Deg2Rad * angle) * radius, 0f);
        angle += angleStep;

        for (int i = 1; i <= segments; i++)
        {
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, Mathf.Sin(Mathf.Deg2Rad * angle) * radius, 0f);
            Debug.DrawLine(prevPoint, nextPoint, color, duration);
            prevPoint = nextPoint;
            angle += angleStep;
        }
    }
}
