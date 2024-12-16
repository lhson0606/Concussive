using System;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public event Action OnPlayerEnterRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerEnterRoom?.Invoke();
            // Disable the trigger collider so that the event is only fired once
            GetComponent<Collider2D>().enabled = false;
        }        
    }
}
