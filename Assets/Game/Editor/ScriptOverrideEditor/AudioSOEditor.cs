using EditYourNameSpace;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioSO))]
// TODO: rename class to AudioSOEditor (and rename this file to match)
public class NewBehaviourScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AudioSO audioSO = (AudioSO)target;

        if (GUILayout.Button("Gather Assets"))
        {
            audioSO.GatherAssets();
        }
    }
}