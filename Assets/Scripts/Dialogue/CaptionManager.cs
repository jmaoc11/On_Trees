using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaptionManager : MonoBehaviour
{
    [SerializeField] private TMP_Text captionUIElement; // Changed from Text to TMP_Text
    [SerializeField] private List<CaptionData> captions;
    [SerializeField] private AudioSource audioSource;
    
    private CaptionData currentCaption;
    private int currentSegmentIndex;
    private bool isPlaying;

    private void Start()
    {
        captionUIElement.text = ""; // Clear caption text initially
    }

    private void Update()
    {
        if (isPlaying && Application.isPlaying) // Only in Play mode
        {
            UpdateCaptions();
        }
    }

    public void PlayCaptionedAudio(int captionIndex)
    {
        if (captionIndex < 0 || captionIndex >= captions.Count) return;

        currentCaption = captions[captionIndex];
        currentSegmentIndex = 0;
        
        // Play the audio
        audioSource.clip = currentCaption.audioClip;
        audioSource.Play();
        
        isPlaying = true;
        UpdateCaptions();
    }

    private void UpdateCaptions()
    {
        if (audioSource == null || currentCaption == null || captionUIElement == null) return;

        float currentTime = audioSource.time;
        
        // Find the appropriate caption segment for the current time
        CaptionSegment currentSegment = null;
        
        if (currentCaption.segments != null)
        {
            foreach (var segment in currentCaption.segments)
            {
                // Check if current time is within this segment's time range
                if (currentTime >= segment.startTime && currentTime <= segment.endTime)
                {
                    currentSegment = segment;
                    break;
                }
            }
        }

        // Update the caption text and force refresh
        if (currentSegment != null)
        {
            captionUIElement.text = currentSegment.text;
            captionUIElement.SetAllDirty(); // Force UI refresh
        }
        else
        {
            captionUIElement.text = "";
            captionUIElement.SetAllDirty(); // Force UI refresh
        }

        // Force the UI to update
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(captionUIElement);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        // Check if audio has finished playing
        if (!audioSource.isPlaying)
        {
            isPlaying = false;
            captionUIElement.text = "";
            captionUIElement.SetAllDirty(); // Force UI refresh
            Debug.Log("Audio finished playing");
        }

        // Debug logging
        Debug.Log($"Time: {currentTime:F2}, Text: {captionUIElement.text}");
    }

    public void PauseAudio()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
            isPlaying = false;
        }
    }

    public void StopAudio()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            isPlaying = false;
            captionUIElement.text = ""; // Clear the caption text
        }
    }

    public void EditorUpdate()
    {
        if (isPlaying)
        {
            UpdateCaptions();
        }
    }

    public CaptionData GetCaptionDataByID(string narrationID)
    {
        return captions.Find(c => c.narrationID == narrationID);
    }

    public int GetCaptionIndex(CaptionData data)
    {
        return captions.IndexOf(data);
    }
}
