using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace Com.A9.AudioManager
{
    [CustomEditor(typeof(AudioConfig))]
    public class AudioConfigEditor : Editor
    {
        static AudioConfig ad_config;
        private void OnEnable()
        {
            ad_config = (AudioConfig)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Absorb"))
            {
                Absorb();
            }
        }

        public static void Absorb()
        {
            List<AudioClip> cps = new List<AudioClip>();
            DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/Audio/Music");
            string[] subdirectory = Directory.GetDirectories(Application.dataPath + "/Resources/Audio/Music");
            subdirectory.Append(Application.dataPath + "/Resources/Audio/Music");
            subdirectory.ToList().ForEach(c =>
            {
                FileInfo[] info = new DirectoryInfo(c).GetFiles("*.*");
                foreach (FileInfo f in info)
                {
                    // input.Substring(input.IndexOf('.') + 1);
                    // Debug.Log(f.FullName.Substring(f.FullName.IndexOf("\\Assets")+1));
                    AudioClip cp = AssetDatabase.LoadAssetAtPath<AudioClip>(f.FullName.Substring(f.FullName.IndexOf("\\Assets") + 1));
                    if (cp)
                        cps.Add(cp);
                }
            }
            );

            if (ad_config == null)
            {
                return;
            }
            for (int i = ad_config.infos.Count - 1; i >= 0; i--)
            {
                if (ad_config.infos[i].clip == null)
                    ad_config.infos.RemoveAt(i);
            }
            for (int i = 0; i < cps.Count; i++)
            {
                SoundData has = ad_config.infos.Find(c => c.clip == cps[i]);
                if (has == null)
                {
                    SoundData d = new SoundData();
                    d.Name = cps[i].name;
                    d.clip = cps[i];
                    ad_config.infos.Add(d);
                }
                else
                {
                    if (has.Name != cps[i].name)
                        has.Name = cps[i].name;
                }
            }
            EditorUtility.SetDirty(ad_config);
        }
    }
}

