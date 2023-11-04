using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditorInternal;
using System;

namespace Com.A9.AudioManager{
public static class AudioManagerShortCut
{
#if UNITY_EDITOR
    [MenuItem("My Commands/Special Command_AbsorbAudio _%a")]
    static void SpecialCommand_AbsorbAudio()
    {
        if (Application.isPlaying)
            return;
        AudioConfig config = Resources.Load<AudioConfig>("AudioConfig/AudioConfig");

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = config;
        AudioConfigEditor.Absorb();

        //AudioClip default_cp = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Extension/Audio/Music/button.mp3", typeof(AudioClip));
        ////Selection.activeObject = default_cp;
        //EditorUtility.FocusProjectWindow();
    }
#endif
}
}
