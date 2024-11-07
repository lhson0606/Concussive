using UnityEngine;

using System.Collections.Generic;

public class SlowMotionManager : MonoBehaviour
{
    public float slowMotionFactor = 0.1f;
    private float originalTimeScale;

    private readonly List<SlowMotionObject> slowMotionObjects = new List<SlowMotionObject>();

    private static SlowMotionManager _instance;

    public static SlowMotionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SlowMotionManager>();

                if (_instance == null)
                {
                    Debug.LogError("SlowMotionManager: No instance found in the scene.");
                }
            }
            return _instance;
        }
    }

    public void ActivateSlowMotion()
    {
        originalTimeScale = Time.timeScale;
        Time.timeScale = slowMotionFactor;

        foreach (var slowMotionObject in slowMotionObjects)
        {
            slowMotionObject.ActivateSlowMotion();
        }
    }

    public void DeactivateSlowMotion()
    {
        Time.timeScale = originalTimeScale;

        foreach (var slowMotionObject in slowMotionObjects)
        {
            slowMotionObject.DeactivateSlowMotion();
        }
    }

    public void RegisterSlowMotionObject(SlowMotionObject slowMotionObject)
    {
        slowMotionObjects.Add(slowMotionObject);
    }

    public void UnregisterSlowMotionObject(SlowMotionObject slowMotionObject)
    {
        slowMotionObjects.Remove(slowMotionObject);
    }
}
