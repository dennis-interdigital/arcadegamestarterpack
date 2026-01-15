using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

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

    #region Debug Toggle
    const string DEFINE_DEBUG = "USE_DEBUG";
    [MenuItem("InterDigital/Toggle USE_DEBUG")]
    public static void ToggleUseDebug()
    {
        BuildTargetGroup group = GetBuildTargetGroup();
        string[] defines = GetDefines(group);

        bool hasDebugDefine = defines.Contains(DEFINE_DEBUG);
        if (hasDebugDefine)
        {
            RemoveDebugDefine(group);
        }
        else
        {
            AddDebugDefine(group);
        }
    }

    [MenuItem("InterDigital/Toggle USE_DEBUG", true)]
    public static bool ToggleDebugValidate()
    {
        BuildTargetGroup group = GetBuildTargetGroup();
        bool enabled = GetDefines(group).Contains(DEFINE_DEBUG);
        Menu.SetChecked("InterDigital/Toggle USE_DEBUG", enabled);
        return true;
    }

    static void AddDebugDefine(BuildTargetGroup group)
    {
        string[] defines = GetDefines(group);
        List<string> list = defines.ToList();

        if (!list.Contains(DEFINE_DEBUG))
        {
            list.Add(DEFINE_DEBUG);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(';', list));
        }
    }

    static void RemoveDebugDefine(BuildTargetGroup group)
    {
        string[] defines = GetDefines(group).Where(d => d != DEFINE_DEBUG).ToArray();
        List<string> list = defines.ToList();

        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(';', list));
    }

    static BuildTargetGroup GetBuildTargetGroup()
    {
        return BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
    }

    static string[] GetDefines(BuildTargetGroup group)
    {
        string[] result = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(':').Where(d => !string.IsNullOrEmpty(d)).ToArray();
        return result;
    }
    #endregion
}