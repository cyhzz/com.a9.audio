using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
namespace Com.A9.AudioManager{
[System.Serializable]
public enum PlayType
{
    Additive, Static, New
}

[System.Serializable]
public class SoundData
{
    public string Name;
    public AudioClip clip;
    public AudioMixerGroup mixer;
    [HideInInspector]
    public float oriVolume;
    [Range(0f, 1f)]
    public float volume = 1.0f;
    [Range(0.5f, 3f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume = 0f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0f;
    public bool loop = false;

    public PlayType playType = PlayType.New;
}
public class AudioManager
{
    //public static AudioManager instance;
    static Dictionary<string, SoundData> db = new Dictionary<string, SoundData>();
    static Dictionary<AudioClip, SoundData> db_clip = new Dictionary<AudioClip, SoundData>();
    static List<AudioInstance> sources = new List<AudioInstance>();
    static GameObject sourceInstance;
    static int indiviualSoundMaxLimit = 10;

    static AudioManager()
    {
        //Load Config
        AudioConfig config = Resources.Load<AudioConfig>("AudioConfig/AudioConfig");
        sourceInstance = Resources.Load<GameObject>("AudioConfig/AudioInstance");
        //Debug.Log(config.infos.Count);

        for (int i = 0; i < config.infos.Count; i++)
        {
            db.Add(config.infos[i].Name, config.infos[i]);
            db_clip.Add(config.infos[i].clip, config.infos[i]);
        }
    }

    //public void Absorb()
    //{
    //    AudioClip[] cps = Resources.LoadAll<AudioClip>("Music/");
    //    for (int i = 0; i < cps.Length; i++)
    //    {
    //        SoundData has = db.Find(c => c.clip == cps[i]);
    //        if (has == null)
    //        {
    //            SoundData d = new SoundData();
    //            d.Name = cps[i].name;
    //            d.clip = cps[i];
    //            db.Add(d);
    //        }
    //        else
    //        {
    //            if (has.Name != cps[i].name)
    //                has.Name = cps[i].name;
    //        }
    //    }
    //}

    //void Awake()
    //{
    //	if (instance != null)
    //	{
    //		if (instance != this)
    //			Destroy(gameObject);
    //	}
    //	else
    //	{
    //		instance = this;
    //		DontDestroyOnLoad(this);
    //	}
    //}
    public static void RemoveInstance(AudioInstance audio)
    {
        sources.Remove(audio);
    }
    public static bool IsPlaying(AudioClip clip)
    {
        foreach (var item in sources)
        {
            if (clip == item.GetClip())
            {
                if (item.IsPlaying())
                    return true;
            }
        }
        return false;
    }
    public static bool IsPlaying(string clip)
    {
        foreach (var item in sources)
        {
            if (clip == item.GetClip().name)
            {
                if (item.IsPlaying())
                    return true;
            }
        }
        return false;
    }
    public static void PlaySound(AudioClip clip, Vector2 pos = new Vector2(), float maxDistance = 5000)
    {
        if (db_clip.ContainsKey(clip))
        {
            SoundData data = db_clip[clip];
            //Check if already exist
            AudioInstance ins = sources.Find(c => c.GetClip() == data.clip);
            if (ins == null)
            {
                GameObject go = GameObject.Instantiate(sourceInstance);
                var ao = go.GetComponent<AudioSource>();
                go.GetComponent<AudioSource>().maxDistance = maxDistance;
                go.transform.position = pos;

                ins = go.GetComponent<AudioInstance>();
                ins.Init(data);
                sources.Add(ins);
                ins.Play();
            }
            else
            {
                if (data.playType == PlayType.Additive)
                {
                    ins.GetComponent<AudioSource>().maxDistance = maxDistance;
                    ins.transform.position = pos;
                    ins.Play();
                }
                else if (data.playType == PlayType.New)
                {
                    //New Filter 
                    int count = 0;
                    AudioInstance newIns = null;
                    foreach (var item in sources)
                    {
                        if (data.clip == item.GetClip())
                        {
                            if (!item.IsPlaying())
                                newIns = item;
                            else
                                count++;
                        }
                    }
                    if (newIns != null)
                    {
                        newIns.GetComponent<AudioSource>().maxDistance = maxDistance;
                        newIns.transform.position = pos;
                        newIns.Play();
                    }
                    else
                    {
                        if (count < indiviualSoundMaxLimit)
                        {
                            GameObject go = GameObject.Instantiate(sourceInstance);

                            go.GetComponent<AudioSource>().maxDistance = maxDistance;
                            go.transform.position = pos;

                            ins = go.GetComponent<AudioInstance>();
                            ins.Init(data);
                            sources.Add(ins);
                            ins.Play();
                        }
                        else
                            Debug.Log("Sound instance overflow" + data.clip.name);
                    }
                }
                else if (!ins.IsPlaying())
                {
                    ins.GetComponent<AudioSource>().maxDistance = maxDistance;
                    ins.transform.position = pos;
                    ins.Play();
                }
            }
        }
        else
            Debug.Log("Sound " + clip.name + " not founded");
    }
    public static void PlaySound(string Name, Vector2 pos = new Vector2(), float maxDistance = 5000, float speed = 1)
    {
        if (Name == "" || Name == "none")
        {
            return;
        }
        // SoundData data = db.Find(c => c.clip.name == Name);
        if (db.ContainsKey(Name))
        {
            SoundData data = db[Name];
            //Check if already exist
            AudioInstance ins = sources.Find(c => c.GetClip() == data.clip);
            if (ins == null)
            {
                GameObject go = GameObject.Instantiate(sourceInstance);
                AudioSource source = go.GetComponent<AudioSource>();
                ins = go.GetComponent<AudioInstance>();

                source.maxDistance = maxDistance;
                source.pitch *= speed;
                go.transform.position = pos;

                ins.Init(data);
                sources.Add(ins);
                ins.Play();
            }
            else
            {
                if (data.playType == PlayType.Additive)
                {
                    var source = ins.GetComponent<AudioSource>();
                    source.maxDistance = maxDistance;
                    source.pitch *= speed;

                    ins.transform.position = pos;
                    ins.Play();
                    source.volume = data.volume;
                    source.loop = data.loop;
                }
                else if (data.playType == PlayType.New)
                {
                    //New Filter 
                    int count = 0;
                    AudioInstance newIns = null;
                    foreach (var item in sources)
                    {
                        if (data.clip == item.GetClip())
                        {
                            if (!item.IsPlaying())
                                newIns = item;
                            else
                                count++;
                        }
                    }
                    if (newIns != null)
                    {
                        var source = newIns.GetComponent<AudioSource>();
                        source.maxDistance = maxDistance;
                        source.pitch *= speed;

                        newIns.transform.position = pos;
                        newIns.Play();
                        source.volume = data.volume;
                        source.loop = data.loop;
                    }
                    else
                    {
                        if (count < indiviualSoundMaxLimit)
                        {
                            GameObject go = GameObject.Instantiate(sourceInstance);
                            var source = go.GetComponent<AudioSource>();
                            ins = go.GetComponent<AudioInstance>();
                            source.maxDistance = maxDistance;
                            source.pitch *= speed;

                            go.transform.position = pos;

                            ins.Init(data);
                            sources.Add(ins);
                            ins.Play();
                        }
                        else
                        {
                            //Debug.Log("Sound instance overflow" + data.clip.name);
                        }
                    }
                }
                else if (!ins.IsPlaying())
                {
                    var source = ins.GetComponent<AudioSource>();
                    source.maxDistance = maxDistance;
                    source.pitch *= speed;

                    ins.transform.position = pos;
                    ins.Play();
                    source.volume = data.volume;
                    source.loop = data.loop;
                }
            }
        }
        else
            Debug.LogWarning("Sound " + Name + " not founded");
    }
    public static void PlaySound(AudioClip clip, PlayType type, Vector2 pos = new Vector2(), float maxDistance = 5000)
    {
        if (db_clip.ContainsKey(clip))
        {
            SoundData data = db_clip[clip];
            //Check if already exist
            AudioInstance ins = sources.Find(c => c.GetClip() == data.clip);
            if (ins == null)
            {
                GameObject go = GameObject.Instantiate(sourceInstance);

                go.GetComponent<AudioSource>().maxDistance = maxDistance;
                go.transform.position = pos;

                ins = go.GetComponent<AudioInstance>();
                ins.Init(data);
                sources.Add(ins);
                ins.Play();
            }
            else
            {
                if (type == PlayType.Additive)
                {
                    ins.Play();
                    ins.GetComponent<AudioSource>().maxDistance = maxDistance;
                    ins.transform.position = pos;
                }
                else if (type == PlayType.New)
                {
                    //New Filter 
                    int count = 0;
                    AudioInstance newIns = null;
                    foreach (var item in sources)
                    {
                        if (data.clip == item.GetClip())
                        {
                            if (!item.IsPlaying())
                                newIns = item;
                            else
                                count++;
                        }
                    }
                    if (newIns != null)
                    {
                        newIns.GetComponent<AudioSource>().maxDistance = maxDistance;
                        newIns.transform.position = pos;
                        newIns.Play();
                    }
                    else
                    {
                        if (count < indiviualSoundMaxLimit)
                        {
                            GameObject go = GameObject.Instantiate(sourceInstance);
                            go.GetComponent<AudioSource>().maxDistance = maxDistance;

                            go.transform.position = pos;
                            ins = go.GetComponent<AudioInstance>();

                            ins.Init(data);
                            sources.Add(ins);
                            ins.Play();
                        }
                        else
                            Debug.Log("Sound instance overflow" + data.clip.name);
                    }
                }
                else if (!ins.IsPlaying())
                {
                    ins.GetComponent<AudioSource>().maxDistance = maxDistance;
                    ins.transform.position = pos;
                    ins.Play();
                }
            }
        }
        else
            Debug.Log("Sound " + clip.name + " not founded");
    }

    public static void StopSoundStatic(AudioClip clip)
    {
        AudioInstance ins = sources.Find(c => c.GetClip() == clip);
        if (ins != null)
            ins.Stop();
    }
    public static void StopSoundStatic(string soundName)
    {
        AudioInstance ins = sources.Find(c => c.GetClip().name == soundName);
        if (ins != null)
            ins.Stop();
    }
    public static AudioInstance GetSoundStatic(string soundName)
    {
        AudioInstance ins = sources.Find(c => c.GetClip().name == soundName);
        return ins;
    }
}

}

