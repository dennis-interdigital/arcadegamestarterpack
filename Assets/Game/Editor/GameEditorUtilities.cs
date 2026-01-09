using System.IO;
using UnityEditor;
using UnityEngine;

public class GameEditorUtilities
{
    [MenuItem("InterDigital/ClearUserData")]
    public static void ClearUserData()
    {
        PlayerPrefs.DeleteAll();
        string persistentDataPath = Application.persistentDataPath;
        if (Directory.Exists(persistentDataPath))
        {
            Directory.Delete(persistentDataPath, true);
        }
        Debug.Log("InterDigital/ClearUserData: All PlayerPrefs and local data are deleted!");
    }

    [MenuItem("InterDigital/GatherAudioClips")]
    public static void GatherAudioClips()
    {
        string bgmPath = "Assets/Game/Audio/BGM/";
        string sfxPath = "Assets/Game/Audio/SFX/";
        string AudioEnumPath = "Assets/Game/Scripts/Utils/AudioEnum.cs";
        string audioSOPath = "Assets/Game/Audio/AudioSO.asset";

        string[] bgmGuids = AssetDatabase.FindAssets("t:audioClip", new[] { bgmPath });
        string[] sfxGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { sfxPath });

        int count = bgmGuids.Length;
        string[] bgmEnumStrings = new string[count];
        AudioClip[] bgmClips = new AudioClip[count];
        for (int i = 0; i < count; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(bgmGuids[i]);
            bgmClips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            string bgmName = bgmClips[i].name.Split('_')[1];
            bgmEnumStrings[i] = bgmName;
        }

        string bgmEnumJoinString = string.Join(",\n\t", bgmEnumStrings);
        if (count > 0) bgmEnumJoinString += ",";

        count = sfxGuids.Length;
        string[] sfxEnumStrings = new string[count];
        AudioClip[] sfxClips = new AudioClip[count];
        for (int i = 0; i < count; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(sfxGuids[i]);
            sfxClips[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            string sfxName = sfxClips[i].name.Split('_')[1];

            sfxEnumStrings[i] = sfxName;
        }

        //ConvertToEnumScript
        string sfxEnumJoinString = string.Join(",\n\t", sfxEnumStrings);
        if (count > 0) sfxEnumJoinString += ",";

        string sfxScriptString =
            "//Note: This file is auto-generated. DO NOT MODIFY!!!!\n" +
            "public enum BGM\n" +
            "{\n\t" +
            bgmEnumJoinString +
            "\n\tCOUNT\n" +
            "}" +
            "\n" +
            "public enum SFX\n" +
            "{\n\t" +
            sfxEnumJoinString +
            "\n\tCOUNT\n" +
            "}";

        File.WriteAllText(AudioEnumPath, sfxScriptString);

        //Assign clips to SO
        AudioSO audioSO = AssetDatabase.LoadAssetAtPath<AudioSO>(audioSOPath);
        audioSO.bgmDatas = new BGMData[bgmClips.Length];
        audioSO.sfxDatas = new SFXData[sfxClips.Length];

        count = audioSO.bgmDatas.Length;
        for (int i = 0; i < count; i++)
        {
            BGMData data = new BGMData();
            data.name = (BGM)i;
            data.clip = bgmClips[i];
            audioSO.bgmDatas[i] = data;
        }

        count = audioSO.sfxDatas.Length;
        for (int i = 0; i < count; i++)
        {
            SFXData data = new SFXData();
            data.name = (SFX)i;
            data.clip = sfxClips[i];
            audioSO.sfxDatas[i] = data;
        }
        Debug.Log("InterDigital/GatherAudioClips: Audio assets assigned!");
    }
}