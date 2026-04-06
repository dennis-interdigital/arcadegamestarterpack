using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
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
        NamedBuildTarget target = GetNamedBuildTarget();
        string[] defines = GetDefines(target);

        bool hasDebugDefine = defines.Contains(DEFINE_DEBUG);
        if (hasDebugDefine)
        {
            RemoveDebugDefine(target);
        }
        else
        {
            AddDebugDefine(target);
        }
    }

    [MenuItem("InterDigital/Toggle USE_DEBUG", true)]
    public static bool ToggleDebugValidate()
    {
        NamedBuildTarget target = GetNamedBuildTarget();
        bool enabled = GetDefines(target).Contains(DEFINE_DEBUG);
        Menu.SetChecked("InterDigital/Toggle USE_DEBUG", enabled);
        return true;
    }

    static void AddDebugDefine(NamedBuildTarget target)
    {
        string[] defines = GetDefines(target);
        List<string> list = defines.ToList();

        if (!list.Contains(DEFINE_DEBUG))
        {
            list.Add(DEFINE_DEBUG);
            PlayerSettings.SetScriptingDefineSymbols(target, string.Join(';', list));
        }
    }

    static void RemoveDebugDefine(NamedBuildTarget target)
    {
        string[] defines = GetDefines(target).Where(d => d != DEFINE_DEBUG).ToArray();
        List<string> list = defines.ToList();

        PlayerSettings.SetScriptingDefineSymbols(target, string.Join(';', list));
    }

    static NamedBuildTarget GetNamedBuildTarget()
    {
        BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
        return NamedBuildTarget.FromBuildTargetGroup(group);
    }

    static string[] GetDefines(NamedBuildTarget target)
    {
        string[] result = PlayerSettings.GetScriptingDefineSymbols(target).Split(';').Where(d => !string.IsNullOrEmpty(d)).ToArray();
        return result;
    }
    #endregion
}