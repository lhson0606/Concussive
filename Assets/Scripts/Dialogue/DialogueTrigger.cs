using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour{

    private bool isFirstTime;

    [Header("First Ink JSon")]
    [SerializeField] private TextAsset firstInkJSon;

    [Header("Defaut Ink JSon")]
    [SerializeField] private TextAsset defautInkJSon;

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
        isFirstTime = true;
    }
    public void TriggerDialogue()
    {
        GameObject gameObject = this.gameObject;
        if(defautInkJSon == null){
            DialogueManager.GetInstance().EnterDialogueMode(firstInkJSon);
        }
        else    
            if(isFirstTime){
                DialogueManager.GetInstance().EnterDialogueMode(firstInkJSon);
                isFirstTime = false;
            }
            else{
                DialogueManager.GetInstance().EnterDialogueMode(defautInkJSon);
            }
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