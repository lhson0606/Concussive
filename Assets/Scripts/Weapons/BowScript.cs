using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class BowScript : BaseWeapon
{
    [SerializeField]
    private GameObject arrowPrefab;
    private GameObject arrow;
    private bool isCharging = false;

    protected override void Start()
    {
        base.Start();
        base.ShouldAlterRenderOrder = false;
    }

    public override void DoAttack()
    {
        animator?.SetBool("IsCharging", true);
        isCharging = true;
        Debug.Log("Charging attack");
    }

    public override void SetAsMainWeapon(BaseCharacter owner)
    {
        base.SetAsMainWeapon(owner);
        base.ShouldAlterRenderOrder = false;
        // set render order to character +1
        weaponSpriteRenderer.sortingOrder = owner.GetCharacterSpriteRenderer().sortingOrder + 1;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void ReleaseAttack()
    {
        if(isCharging)
        {
            isCharging = false;
            // destroy the arrow
            Destroy(arrow);
            
        }

        animator?.SetBool("IsCharging", false);
        Debug.Log("Releasing attack");

        if (arrow)
        {
            // release the arrow
            ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
            arrowScript.OnProjectileRelease();
            arrow = null;
        }        
    }

    // This method is called from the animator
    public void SpawnArrow()
    {
        isCharging = false;
        // get the bow rotation
        float angle = transform.rotation.eulerAngles.z;
        // spawn the arrow
        arrow = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, 0, angle), transform);
        //rotate the arrow to match the bow
        arrow.transform.Rotate(0, 0, -90);
        ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
        arrowScript.OnProjectileSpawn();
    }

    public void OnArrowFullyCharged()
    {
        if (arrow)
        {
            arrow.GetComponent<ArrowScript>().OnProjectileFullyCharged();
        }
    }
}
