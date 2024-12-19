using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class BowScript : BaseWeapon
{
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private AudioClip arrowNockSound;
    [SerializeField]
    private AudioClip arrowReleaseSound;

    private GameObject arrow;
    private bool isCharging = false;

    protected override void Start()
    {
        base.Start();
        base.ShouldAlterRenderOrder = false;
        audioSource = GetComponent<AudioSource>();
    }

    public override void DoAttack()
    {
        if(isCharging)
        {
            return;
        }

        animator?.SetBool("IsCharging", true);
        isCharging = true;
    }

    public override void SetAsMainWeapon(BaseCharacter owner)
    {
        base.SetAsMainWeapon(owner);
        base.ShouldAlterRenderOrder = false;
        // set render order to character +1
        weaponSpriteRenderer.sortingOrder = owner.GetCharacterSpriteRenderer().sortingOrder + 1;
    }

    public override void ReleaseAttack()
    {
        if(isCharging)
        {
            isCharging = false;
            // destroy the arrow
            if(arrow != null)
            {
                Destroy(arrow);
            }                    
        }

        animator?.SetBool("IsCharging", false);

        if (arrow)
        {
            // release the arrow
            ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
            arrowScript.OnProjectileRelease();
            arrow = null;

            if(arrowReleaseSound)
            {
                audioSource?.PlayOneShot(arrowReleaseSound);
            }
        }
    }

    // This method is called from the animator
    public void SpawnArrow()
    {
        if(arrow != null)
        {
            Destroy(arrow);
            arrow = null;
        }

        isCharging = false;
        // get the bow rotation
        float angle = transform.rotation.eulerAngles.z;
        // spawn the arrow
        arrow = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, 0, angle), transform);
        //rotate the arrow to match the bow
        arrow.transform.Rotate(0, 0, -90);
        ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
        arrowScript.OnProjectileSpawn(owner, this);

        if(arrowNockSound)
        {
            audioSource?.PlayOneShot(arrowNockSound);
        }
    }

    public void OnArrowFullyCharged()
    {
        if (arrow)
        {
            arrow.GetComponent<ArrowScript>().OnProjectileFullyCharged();
        }
    }

    public override void OnEquippedAsOffHandWeapon()
    {
        base.OnEquippedAsOffHandWeapon();
        // cancel the charging attack
        isCharging = false;
        animator?.SetBool("IsCharging", false);

        if (arrow)
        {
            Destroy(arrow);
        }
    }
}
