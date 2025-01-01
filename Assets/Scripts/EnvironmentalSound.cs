using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(AudioSource))]
public class EnvironmentalSound : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip audioClip;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;
    [Range(0.5f, 1.5f)]
    [SerializeField] private float pitch = 1f;
    [SerializeField] private bool randomizeStartTime = true;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.spatialBlend = 0f; // 2D sound by default

        if (audioClip != null)
        {
            if (randomizeStartTime)
            {
                audioSource.time = Random.Range(0f, audioClip.length);
            }
            audioSource.Play();
        }
    }

    private void OnValidate()
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
            audioSource.pitch = pitch;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnvironmentalSound))]
public class EnvironmentalSoundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(5);
        
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioClip"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pitch"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomizeStartTime"));

            if (serializedObject.FindProperty("audioClip").objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign an audio clip to play.", MessageType.Info);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
