using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class ExitWay : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    private GameObject textPrefab;
    private GameObject interactionText;

    private void Awake()
    {
        textPrefab = Resources.Load<GameObject>("Prefabs/UI/Texts/InteractionText");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(interactionText != null && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(interactionText != null) {
                interactionText = null;
                Destroy(interactionText);
            }

            interactionText = Instantiate(textPrefab, transform.position, Quaternion.identity);
            interactionText.transform.SetParent(transform);
            TextMeshProUGUI text = interactionText.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "Interact (E)";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(interactionText);
            interactionText = null;
        }
    }
}
