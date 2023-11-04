using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace  Com.A9.AudioManager
{
    [CreateAssetMenuAttribute(fileName = "AudioConfig", menuName = "AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        public List<SoundData> infos;
    }
}
