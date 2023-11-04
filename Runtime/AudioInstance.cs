using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Com.A9.AudioManager{
public class AudioInstance : MonoBehaviour
{
    public static float maxWaitingTime = 10f;
    [HideInInspector]
    public SoundData data;
    [HideInInspector]
    public AudioSource source;
    float notPlayedCounter = 0;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    //Called By AudioManager
    public void Init(SoundData data)
    {
        this.data = data;
        name = data.clip.name;
        source.clip = data.clip;
        source.loop = data.loop;
        source.outputAudioMixerGroup = data.mixer;
        source.volume = data.volume * (1 + Random.Range(-data.randomVolume / 2f, data.randomVolume / 2f));
        source.pitch = data.pitch * (1 + Random.Range(-data.randomPitch / 2f, data.randomPitch / 2f));
    }
    //Called By AudioManager
    public void Play()
    {
        source.Play();
        notPlayedCounter = 0;
    }
    //Called By AudioManager
    public void Stop()
    {
        source.Stop();
    }
    //AutoDestroy
    void Update()
    {
        if (!source.isPlaying)
            notPlayedCounter += Time.deltaTime;
        if (notPlayedCounter > maxWaitingTime)
        {
            //AudioManager.RemoveInstance(this);
            Destroy(gameObject);
        }
    }
    public bool IsPlaying()
    {
        return source.isPlaying;
    }
    public AudioClip GetClip()
    {
        return data.clip;
    }
    public PlayType GetPlayType()
    {
        return data.playType;
    }

    private void OnDestroy()
    {
        AudioManager.RemoveInstance(this);
    }
}
}

