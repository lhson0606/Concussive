using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    private GameObject doorClosed;
    private GameObject doorOpen;
    private GameObject doorObj;
    private AudioSource audioSource;
    public AudioClip doorOpenSound = null;
    public float volume = 2.5f;
    public bool isOpen = false;
    private bool hasChangedState = false;
    private bool isInGame = false;
    private bool isPlayerInRange = false;
    public float interactionCooldownInSecond = 1f;
    private float interactionSecondsRemaining = 0f;
    private GameObject interactionText;
    private PlayerController playerController = null;
    private bool playerInteracted = false;
    public float delayPlayerTime = 0.5f;

    private void OnValidate()
    {
        hasChangedState = true;
    }

    private void UpdateDoorState()
    {
        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }

        hasChangedState = false;
    }

    private void OpenDoor()
    {
        doorClosed.SetActive(false); // Make doorClosed inactive
        doorOpen.SetActive(true);    // Make doorOpen active
        isOpen = true;
        if (doorOpenSound != null && isInGame)
        {
            audioSource.volume = volume; // Set volume to 1.5 times the default
            audioSource.PlayOneShot(doorOpenSound);
        }
    }

    private void InitializeDoors()
    {
        doorObj = gameObject;
        Transform doorClosedTransform = doorObj.transform.Find("doors_leaf_closed");
        Transform doorOpenTransform = doorObj.transform.Find("doors_leaf_open");

        if (doorClosedTransform == null)
        {
            Debug.LogError("DoorController: doorObj does not have a child named doors_leaf_closed");
            return;
        }
        else if (doorOpenTransform == null)
        {
            Debug.LogError("DoorController: doorObj does not have a child named doors_leaf_open");
            return;
        }
        else
        {
            doorClosed = doorClosedTransform.gameObject;
            doorOpen = doorOpenTransform.gameObject;
        }

        // Get the AudioSource component
        audioSource = doorObj.GetComponent<AudioSource>();
        if (audioSource == null && isInGame)
        {
            Debug.LogError("DoorController: No AudioSource component found on doorObj");
        }

        // Find the Text component
        Transform canvasTransform = doorObj.transform.Find("Canvas");

        if(canvasTransform == null)
        {
            Debug.LogError("DoorController: No child named 'Canvas' found on doorObj");
            return;
        }

        Transform textTransform = canvasTransform.Find("InteractionText");

        if (textTransform != null)
        {
            interactionText = textTransform.gameObject;
        }
        else
        {
            Debug.LogError("DoorController: No child named 'InteractionText' found on doorObj");
        }
    }

    private void CloseDoor()
    {
        doorClosed.SetActive(true);  // Make doorClosed active
        doorOpen.SetActive(false);   // Make doorOpen inactive
        isOpen = false;
        if (doorOpenSound != null && isInGame)
        {
            audioSource.volume = volume; // Set volume to 1.5 times the default
            audioSource.PlayOneShot(doorOpenSound);
        }
    }

    private void Awake()
    {
        InitializeDoors();
        UpdateDoorState();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isInGame = true;
        playerController = GameObject.FindFirstObjectByType<PlayerController>();
        //if player doesn't have a valid player controller, log an error
        if (playerController == null)
        {
            Debug.LogError("DoorController: No PlayerController found in the scene");
        }
        //check if player has multiple player controllers, if so, log an error
        if(FindObjectsByType<PlayerController>(FindObjectsSortMode.None).Length > 1)
        {
            Debug.LogError("DoorController: Multiple PlayerControllers found in the scene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update the cooldown timer
        if (interactionSecondsRemaining > 0)
        {
            interactionSecondsRemaining -= Time.deltaTime;
        }

        if (IsInteractable() && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            hasChangedState = true;
            interactionSecondsRemaining = interactionCooldownInSecond;
            playerInteracted = true;
        }

        UpdateInteractionTextState();

        if (hasChangedState && isInGame)
        {
            if(playerInteracted && playerController != null)
            {
                playerInteracted = false;
                hasChangedState = false;
                //for game
                StartCoroutine(DisableInputTemporarilyAndUpdateDoorState(delayPlayerTime));
            }
            else
            {
                // for editor
                UpdateDoorState();
            }            
        }
    }

    private IEnumerator DisableInputTemporarilyAndUpdateDoorState(float delayPlayerTime)
    {
        playerController.SetEnabled(false);
        playerController.ResetMovement();
        yield return new WaitForSeconds(delayPlayerTime);
        playerController.SetEnabled(true);
        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private bool IsInteractable()
    {
        return interactionSecondsRemaining <= 0 && isPlayerInRange;
    }


    private void UpdateInteractionTextState()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(IsInteractable());
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

