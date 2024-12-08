using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Effect : SlowMotionObject
{
    public string effectName = "?";
    public float duration = 0f;
    public float tickRate = 0f;
    public float delay = 0f;
    public ParticleSystem particles = null;
    public EffectType effectType = EffectType.NONE;
    [SerializeField]
    protected AudioClip effectStartAudio;


    protected BaseCharacter target;
    protected ParticleSystem particlesInstance;
    protected AudioSource audioSource;
    protected Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void StartEffect(BaseCharacter target)
    {
        this.target = target;
        target.AddEffect(this);
        transform.localScale *= target.GetEffectSizeScale();

        // Debug logs to check the values
        Debug.Log($"Starting effect: {effectName} with delay: {delay}, tickRate: {tickRate}, duration: {duration}");

        InvokeRepeating("ApplyEffect", delay, tickRate);
        Invoke("EndEffect", duration);

        transform.SetParent(target.transform);

        if (particles != null)
        {
            // spawn particles at the target's position
            particlesInstance = Instantiate(particles, target.transform.position, Quaternion.identity,transform);
            particlesInstance.transform.localScale *= target.GetEffectSizeScale();

            var mainModule = particlesInstance.main;
            mainModule.duration = duration;
            mainModule.loop = true;
            particlesInstance.Play();
        }

        if(effectStartAudio != null)
        {
            audioSource.PlayOneShot(effectStartAudio);
        }
    }

    public virtual void ApplyEffect()
    {
        // Debug log to verify ApplyEffect is being called
        Debug.Log($"Applying effect: {effectName}");
        // Implement the effect logic here
    }

    public virtual void OnEffectEnd()
    {
        // Implement the logic to be executed
    }

    public virtual void EndEffect()
    {
        Debug.Log($"Ending effect: {effectName}");
        CancelInvoke("ApplyEffect");
        target.RemoveEffect(this);

        // Stop and destroy the particle system
        if (particlesInstance != null)
        {
            particlesInstance.Stop();
            Destroy(particlesInstance.gameObject, particlesInstance.main.duration);
        }
        OnEffectEnd();
        Destroy(this.gameObject);
    }

    public EffectType EffectType => effectType;
}