using UnityEngine;

public class NarrationManager : MonoBehaviour
{
    public static NarrationManager Instance { get; private set; }
    [SerializeField] private CaptionManager captionManager;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Simple method to play narration by ID
    public void PlayNarration(string narrationID)
    {
        Debug.Log($"Attempting to play narration: {narrationID}"); // Debug log
        
        var captionData = captionManager.GetCaptionDataByID(narrationID);
        if (captionData != null)
        {
            Debug.Log("Found caption data"); // Debug log
            int index = captionManager.GetCaptionIndex(captionData);
            captionManager.PlayCaptionedAudio(index);
        }
        else
        {
            Debug.Log($"No caption data found for ID: {narrationID}"); // Debug log
        }
    }
}
