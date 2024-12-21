using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CaptionManager))]
public class CaptionManagerEditor : Editor
{
    private bool isPlaying = false;
    private double lastTimeSinceStartup;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CaptionManager manager = (CaptionManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Caption Testing", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Test Audio Controls", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        // Play/Pause Button
        if (GUILayout.Button(isPlaying ? "Pause" : "Play Caption 0"))
        {
            if (!isPlaying)
            {
                manager.PlayCaptionedAudio(0);
                isPlaying = true;
                lastTimeSinceStartup = EditorApplication.timeSinceStartup;
                EditorApplication.update += OnEditorUpdate;
            }
            else
            {
                manager.PauseAudio();
                isPlaying = false;
                EditorApplication.update -= OnEditorUpdate;
            }
        }

        // Stop Button
        if (GUILayout.Button("Stop"))
        {
            manager.StopAudio();
            isPlaying = false;
            EditorApplication.update -= OnEditorUpdate;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void OnEditorUpdate()
    {
        if (isPlaying)
        {
            CaptionManager manager = (CaptionManager)target;
            manager.EditorUpdate();
            
            // Force UI to update
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            Repaint();
        }
    }

    void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }
}
