using System;
using UnityEngine;

[Serializable]
public class BGMData
{
    public BGM name;
    public AudioClip clip;
}

[Serializable]
public class SFXData
{
    public SFX name;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "AudioSO", menuName = "SO/Audio")]
public class AudioSO : ScriptableObject
{
    public BGMData[] bgmDatas;
    public SFXData[] sfxDatas;
}

