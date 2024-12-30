using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class VegetationAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Sprite[] animationFrames;
    [SerializeField] private float frameRate = 12f;
    [SerializeField] private bool randomizeStartFrame = true;
    [SerializeField] private float delayBetweenLoops = 1f; // Delay in seconds between animation loops

    private SpriteRenderer spriteRenderer;
    private float timeElapsed;
    private float loopDelayTimer;
    private int currentFrame;
    private bool isWaitingForNextLoop;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (randomizeStartFrame && animationFrames != null && animationFrames.Length > 0)
        {
            currentFrame = UnityEngine.Random.Range(0, animationFrames.Length);
            spriteRenderer.sprite = animationFrames[currentFrame];
        }
    }

    private void Update()
    {
        AnimateSprite();
    }

    private void AnimateSprite()
    {
        if (animationFrames == null || animationFrames.Length == 0) return;

        if (isWaitingForNextLoop)
        {
            // Return to first frame when starting delay
            if (loopDelayTimer == 0f)
            {
                currentFrame = 0;
                spriteRenderer.sprite = animationFrames[currentFrame];
            }

            loopDelayTimer += Time.deltaTime;
            if (loopDelayTimer >= delayBetweenLoops)
            {
                isWaitingForNextLoop = false;
                loopDelayTimer = 0f;
            }
            return;
        }

        timeElapsed += Time.deltaTime;
        
        if (timeElapsed >= 1f / frameRate)
        {
            timeElapsed = 0f;
            
            // If we're at the last frame, start the delay
            if (currentFrame >= animationFrames.Length - 1)
            {
                isWaitingForNextLoop = true;
                loopDelayTimer = 0f;
                return;
            }

            // Otherwise, advance to next frame
            currentFrame++;
            spriteRenderer.sprite = animationFrames[currentFrame];
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(VegetationAnimator)), CanEditMultipleObjects]
public class VegetationAnimatorEditor : Editor
{
    SerializedProperty animationFrames;
    SerializedProperty frameRate;
    SerializedProperty randomizeStartFrame;
    SerializedProperty delayBetweenLoops;

    private bool showAnimationSettings = true;

    private void OnEnable()
    {
        animationFrames = serializedObject.FindProperty("animationFrames");
        frameRate = serializedObject.FindProperty("frameRate");
        randomizeStartFrame = serializedObject.FindProperty("randomizeStartFrame");
        delayBetweenLoops = serializedObject.FindProperty("delayBetweenLoops");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(10);
        
        using (new EditorGUILayout.VerticalScope())
        {
            showAnimationSettings = EditorGUILayout.Foldout(showAnimationSettings, "Animation Settings", true);
            if (showAnimationSettings)
            {
                EditorGUI.indentLevel++;
                
                // Only show drag-drop area if single object is selected
                if (!serializedObject.isEditingMultipleObjects)
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        GUILayout.Label("Drag and Drop Sprite Sequence Here\n(Will be sorted by name)", 
                            EditorStyles.centeredGreyMiniLabel);
                        
                        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 35.0f, 
                            GUILayout.ExpandWidth(true),
                            GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 40));
                        
                        // Handle drag and drop
                        if (Event.current.type == EventType.DragUpdated && dropArea.Contains(Event.current.mousePosition))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            Event.current.Use();
                        }
                        else if (Event.current.type == EventType.DragPerform && dropArea.Contains(Event.current.mousePosition))
                        {
                            DragAndDrop.AcceptDrag();
                            
                            // Get all sprites from the drag operation
                            UnityEngine.Object[] draggedObjects = DragAndDrop.objectReferences;
                            List<Sprite> sprites = new List<Sprite>();
                            
                            foreach (UnityEngine.Object obj in draggedObjects)
                            {
                                if (obj is Sprite sprite)
                                {
                                    sprites.Add(sprite);
                                }
                                else if (obj is Texture2D texture)
                                {
                                    string path = AssetDatabase.GetAssetPath(obj);
                                    UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                                    foreach (UnityEngine.Object asset in assets)
                                    {
                                        if (asset is Sprite s)
                                        {
                                            sprites.Add(s);
                                        }
                                    }
                                }
                            }
                            
                            sprites.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
                            
                            animationFrames.ClearArray();
                            for (int i = 0; i < sprites.Count; i++)
                            {
                                animationFrames.InsertArrayElementAtIndex(i);
                                animationFrames.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
                            }
                            
                            serializedObject.ApplyModifiedProperties();
                            Event.current.Use();
                        }
                    }
                    
                    EditorGUILayout.Space(5);
                }
                
                // Show properties (these work with multi-select)
                EditorGUILayout.PropertyField(animationFrames);
                EditorGUILayout.PropertyField(frameRate);
                EditorGUILayout.PropertyField(randomizeStartFrame);
                EditorGUILayout.PropertyField(delayBetweenLoops);
                
                if (animationFrames.arraySize == 0)
                {
                    EditorGUILayout.HelpBox("Add sprite frames to create the animation.", MessageType.Info);
                }
                
                EditorGUI.indentLevel--;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif