using UnityEngine;

public class NormalChestController : MonoBehaviour
{
    private Animator chestAnimator;
    private bool isPlayerInRange = false;
    private bool isOpen = false;
    private AudioSource audioSource;
    public GameObject itemToSpawn; // This should be a prefab
    private Vector3 itemSpawnPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chestAnimator = GetComponent<Animator>();

        if (chestAnimator == null)
        {
            Debug.LogError("NormalChestController: No Animator component found on chest object");
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("NormalChestController: No AudioSource component found on chest object");
        }

        Transform spawnPositionTransform = transform.Find("SpawnLocation");

        if (spawnPositionTransform == null)
        {
            Debug.LogError("NormalChestController: No child named 'SpawnLocation' found on chest object");
        }
        else
        {
            itemSpawnPos = spawnPositionTransform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            Debug.Log("Player pressed E to open chest");
            chestAnimator.SetTrigger("Open");
        }
    }

    public void OnOpenAnimationFinished()
    {
        Debug.Log("Chest opened");
        isOpen = true;
        audioSource.Play();
        SpawnItem();
    }

    private void SpawnItem()
    {
        if (itemToSpawn != null)
        {
            Instantiate(itemToSpawn, itemSpawnPos, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player entered chest trigger");
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player exited chest trigger");
            isPlayerInRange = false;
        }
    }
}
