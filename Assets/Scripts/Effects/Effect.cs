using UnityEngine;

public class Effect : SlowMotionObject
{
    public string effectName = "?";
    public float duration = 0f;
    public float tickRate = 0f;
    public float delay = 0f;
    public Sprite icon = null;
    public ParticleSystem particles = null;
    public EffectType effectType = EffectType.NONE;

    protected BaseCharacter target;

    private ParticleSystem particlesInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void StartEffect(BaseCharacter target)
    {
        this.target = target;
        target.AddEffect(this);

        // Debug logs to check the values
        Debug.Log($"Starting effect: {effectName} with delay: {delay}, tickRate: {tickRate}, duration: {duration}");

        if (tickRate > 0)
        {
            InvokeRepeating("ApplyEffect", delay, tickRate);
            Invoke("EndEffect", duration);
        }
        else
        {
            Debug.LogError("Tick rate must be greater than 0");
        }

        if (particles != null)
        {
            // spawn particles at the target's position
            particlesInstance = Instantiate(particles, target.transform.position, Quaternion.identity, target.transform);
            var mainModule = particlesInstance.main;
            mainModule.duration = duration;
            mainModule.loop = true;
            particlesInstance.Play();
        }
    }

    public virtual void ApplyEffect()
    {
        // Debug log to verify ApplyEffect is being called
        Debug.Log($"Applying effect: {effectName}");
        // Implement the effect logic here
    }

    public virtual void EndEffect()
    {
        Debug.Log($"Ending effect: {effectName}");
        CancelInvoke("ApplyEffect");
        target.RemoveEffect(this);
        Destroy(this.gameObject);

        // Stop and destroy the particle system
        if (particlesInstance != null)
        {
            particlesInstance.Stop();
            Destroy(particlesInstance.gameObject, particlesInstance.main.duration);
        }
    }

    public EffectType EffectType => effectType;
}