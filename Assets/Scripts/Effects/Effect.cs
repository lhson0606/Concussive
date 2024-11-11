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

        // Debug logs to check the values
        Debug.Log($"Starting effect: {effectName} with delay: {delay}, tickRate: {tickRate}, duration: {duration}");

        InvokeRepeating("ApplyEffect", delay, tickRate);
        Invoke("EndEffect", duration);

        transform.SetParent(target.transform);

        if (particles != null)
        {
            // spawn particles at the target's position
            particlesInstance = Instantiate(particles, target.transform.position, Quaternion.identity, transform);
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