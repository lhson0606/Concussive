using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour{

    // [Header("VisualCue")]
    // [SerializeField] private GameObject visualCue;

    [Header("Ink JSon")]
    [SerializeField] private TextAsset inkJSon;

    private bool playerInRange;
    public KeyCode interacKey;

    private void Update(){
        if(playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying){
        //     visualCue.SetActive(true);
            if (Input.GetKeyDown(interacKey)){
                TriggerDialogue();
            }
        }
        else{
        //     visualCue.SetActive(false);
        }
    }
    private void Awake(){
        playerInRange = false;
        // visualCue.SetActive(false); 
    }
    public void TriggerDialogue()
    {
        GameObject gameObject = this.gameObject;
        DialogueManager.GetInstance().EnterDialogueMode(inkJSon);
        DialogueManager.GetInstance().SetCurrentGameObject(gameObject);
    }  

    private void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.tag == "Player"){
            playerInRange = true;
        }

    }
    
    private void OnTriggerExit2D(Collider2D collider){
        if (collider.gameObject.tag == "Player"){
            playerInRange = false;
        }
    }
}