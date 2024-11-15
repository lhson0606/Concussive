using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [Header("Ink JSon")]
    [SerializeField] private TextAsset inkJSon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DialogueManager.GetInstance().EnterDialogueMode(inkJSon);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
