using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchFlickeringController : SlowMotionObject
{
    [SerializeField]
    private float amplitude = 0.5f;

    private Light2D light;
    float outerRadius;

    void Start()
    {
        light = GetComponent<Light2D>();
        outerRadius = light.pointLightOuterRadius;
    }

    void Update()
    {
        light.pointLightOuterRadius = outerRadius + GetFlickerValue();
    }

    private float GetFlickerValue()
    {
        return Mathf.Sin(Time.time * 10) * amplitude;
    }
}
