using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Interactable : MonoBehaviour
{
    public bool isInRange;
    public KeyCode interacKey;
    public UnityEvent interacAction;

    private void Awake()
    {
        isInRange = false;
    }
    // Update is called once per frame
    private void Update()
    {
        if (isInRange)
        {
            Debug.Log("In range of " + this.gameObject.name);
            if(interacKey == KeyCode.None) { interacAction.Invoke(); }
            else
            if (Input.GetKeyDown(interacKey))
                interacAction.Invoke();

        }
    }
}


