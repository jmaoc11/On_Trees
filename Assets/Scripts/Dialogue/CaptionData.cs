using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Caption Data", menuName = "Captions/Caption Data")]
public class CaptionData : ScriptableObject
{
    public string narrationID;
    public AudioClip audioClip;
    public List<CaptionSegment> segments = new List<CaptionSegment>();
}

[System.Serializable]
public class CaptionSegment
{
    public string text;
    public float startTime;
    public float endTime;
}
