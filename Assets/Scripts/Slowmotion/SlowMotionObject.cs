using UnityEngine;

public class SlowMotionObject : MonoBehaviour
{
    protected SlowMotionManager slowMotionManager;

    protected virtual void Awake()
    {
        slowMotionManager = SlowMotionManager.Instance;
        slowMotionManager.RegisterSlowMotionObject(this);
    }

    protected virtual void OnDestroy()
    {
        slowMotionManager.UnregisterSlowMotionObject(this);
    }

    public void ActivateSlowMotion()
    {
        // Implement slow-motion effects for this object, e.g., adjusting animation speeds
    }

    public void DeactivateSlowMotion()
    {
        // Restore normal speed for this object
    }
}
