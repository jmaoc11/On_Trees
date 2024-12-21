using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using System.Collections.Generic;

[CustomEditor(typeof(CaptionData))]
public class CaptionDataEditor : Editor
{
    private AudioSource previewSource;
    private float previewTime;
    private bool isPlaying;
    private double lastTimeSinceStartup;

    void OnEnable()
    {
        // Create a temporary GameObject with an AudioSource for preview
        GameObject previewObj = EditorUtility.CreateGameObjectWithHideFlags(
            "AudioPreview", 
            HideFlags.HideAndDontSave, 
            typeof(AudioSource));
        previewSource = previewObj.GetComponent<AudioSource>();

        // Configure the AudioSource
        previewSource.playOnAwake = false;
        previewSource.spatialBlend = 0f; // Make it 2D audio
    }

    void OnDisable()
    {
        // Clean up
        if (previewSource != null)
        {
            if (previewSource.isPlaying)
            {
                previewSource.Stop();
            }
            DestroyImmediate(previewSource.gameObject);
        }
    }

    public override void OnInspectorGUI()
    {
        CaptionData data = (CaptionData)target;
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        
        // Play/Pause Button
        if (GUILayout.Button(isPlaying ? "Pause" : "Play"))
        {
            if (data.audioClip != null)
            {
                if (!isPlaying)
                {
                    // Stop any existing playback
                    if (previewSource.isPlaying)
                    {
                        previewSource.Stop();
                    }
                    
                    // Set up new playback
                    previewSource.clip = data.audioClip;
                    previewSource.time = previewTime;
                    previewSource.Play();
                    lastTimeSinceStartup = EditorApplication.timeSinceStartup;
                }
                else
                {
                    previewSource.Stop();
                }
                isPlaying = !isPlaying;
            }
        }

        // Stop Button
        if (GUILayout.Button("Stop"))
        {
            if (previewSource != null)
            {
                previewSource.Stop();
                previewTime = 0;
                isPlaying = false;
            }
        }

        EditorGUILayout.EndHorizontal();

        // Time slider in its own horizontal group
        EditorGUILayout.BeginHorizontal();
        if (data.audioClip != null)
        {
            // Format the float value to 3 decimal places before using it in the slider
            previewTime = Mathf.Round(previewTime * 1000f) / 1000f;
            float newTime = EditorGUILayout.Slider(previewTime, 0, data.audioClip.length);
            
            if (newTime != previewTime)
            {
                previewTime = Mathf.Round(newTime * 1000f) / 1000f;
                if (isPlaying)
                {
                    previewSource.time = previewTime;
                }
            }
            
            // Format with exactly 3 decimal places
            EditorGUILayout.LabelField($"{data.audioClip.length:F3}s", 
                GUILayout.Width(50));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Add segment at current time button
        if (GUILayout.Button("Add Segment at Current Time"))
        {
            var segment = new CaptionSegment
            {
                startTime = previewTime,
                endTime = previewTime + 2f
            };
            if (data.segments == null)
                data.segments = new List<CaptionSegment>();
                
            data.segments.Add(segment);
            EditorUtility.SetDirty(target);
        }

        DrawDefaultInspector();
        
        serializedObject.ApplyModifiedProperties();
        
        if (isPlaying)
        {
            // Update time display while playing
            if (previewSource.isPlaying)
            {
                // Round the time to 3 decimal places
                previewTime = Mathf.Round(previewSource.time * 1000f) / 1000f;
            }
            Repaint();
        }
    }
}
