using System.Collections.Generic;
using UnityEngine;
public class NarrationTrigger : MonoBehaviour
{
    [SerializeField] private string narrationID;
    [SerializeField] private bool playOnce = true;
    [SerializeField] private bool playOnStart = false;  // Option to play when scene starts
    [SerializeField] private bool useCollisionTrigger = true;  // Toggle for collision detection
    
    private bool hasPlayed = false;

    private void Start()
    {
        if (playOnStart)
        {
            PlayNarration();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered trigger!"); // Basic debug

        if (!useCollisionTrigger) return;
        
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered trigger with ID: {narrationID}");
            PlayNarration();
        }
    }

    public void PlayNarration()
    {
        if (playOnce && hasPlayed) return;
        
        NarrationManager.Instance.PlayNarration(narrationID);
        hasPlayed = true;
    }
}
