using System.IO;
using UnityEditor;
using UnityEngine;
using EditYourNameSpace;

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
}