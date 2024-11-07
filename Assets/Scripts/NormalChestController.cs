using UnityEngine;

public class NormalChestController : SlowMotionObject
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
            chestAnimator.SetTrigger("Open");
        }
    }

    public void OnOpenAnimationFinished()
    {
        isOpen = true;
        audioSource.Play();
        SpawnItem();
    }

    private void SpawnItem()
    {
        if (itemToSpawn != null)
        {
            GameObject spawnedItem = Instantiate(itemToSpawn, itemSpawnPos, Quaternion.identity);
            GameItem gameItem = spawnedItem.GetComponent<GameItem>();

            gameItem.DropItem(itemSpawnPos);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
