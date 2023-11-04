using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Com.A9.AudioManager{
public class PlaySoundEvent : MonoBehaviour
{
    public string sound;
    public void PlaySound()
    {
        string lst = sound.Split(' ').OrderBy(c => UnityEngine.Random.Range(0, 1000)).First();
        AudioManager.PlaySound(lst);
    }
}

}
