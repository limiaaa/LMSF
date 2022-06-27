using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SoundManager : MonoSingleton<SoundManager>
{

    AudioSource Audio_Bgm;
    AudioSource Audio_Effect;
    string AudioPath = "Assets/MainApp/Audio/{0}.mp3";
    Dictionary<string, AudioSource> AudioSourceDic = new Dictionary<string, AudioSource>();
    List<AudioSource> AduioSourceList = new List<AudioSource>();
    protected override void Init()
    {
        base.Init();
        ObjInit();
        AudioListener listener = transform.GetComponent<AudioListener>();
        if (listener == null)
            listener = gameObject.AddComponent<AudioListener>();
    }
    GameObject Effect;
    private void ObjInit()
    {
        GameObject Bgm = new GameObject("Audio_Bgm");
        Bgm.transform.SetParent(transform);
        Bgm.AddComponent<AudioSource>();
        Audio_Bgm = Bgm.GetComponent<AudioSource>();
        Effect = new GameObject("Audio_Effect");
        Effect.transform.SetParent(transform);
        Effect.AddComponent<AudioSource>();
        Audio_Effect = Effect.GetComponent<AudioSource>();
        //AudioClip bgm = ResourceManager.Load<AudioClip>(AudioPath + "bgm.wav");
        //Audio_Bgm.clip = bgm;
        //PlayBGM();
    }

    public void PlayBGM(string name = "bgm_main.wav", bool Loop = true)
    {
        //if (!LocalDataManager.Instance.GetMusicState())
        //    SetBgmVolume(0);
        AudioClip bgm = ResourceManager.Instance.LoadAsset<AudioClip>("",string.Format(AudioPath, name));
        if (bgm != Audio_Bgm.clip)
        {
            Audio_Bgm.clip = bgm;
            Audio_Bgm.loop = Loop;
            Audio_Bgm.Play();
        }
    }
    public void RePlayBGM(string name = "bgm.wav", bool Loop = true)
    {
        //if (!LocalDataManager.Instance.GetMusicState())
        //    SetBgmVolume(0);
        AudioClip bgm = ResourceManager.Instance.LoadAsset<AudioClip>("", string.Format(AudioPath, name));
        Audio_Bgm.clip = bgm;
        Audio_Bgm.loop = Loop;
        Audio_Bgm.Play();
    }


    public void StopBgm()
    {
        Audio_Bgm.Pause();
    }
    public void BgmMute()
    {
        Audio_Bgm.volume = 0;
    }
    public void SetBgmVolume(float vule)
    {
        Audio_Bgm.volume = vule;
    }

    public float GetBgmVolume()
    {
        return Audio_Bgm.volume;
    }
    public void PlaySoundEffect(string name, bool Loop = false)
    {
        PlaySoundMixEffect(name, Loop);
    }
    public void PlaySoundEffectWithNotRepetition(string name)
    {
        //if (!LocalDataManager.Instance.GetSoundState())
        //    SetSoundEffectVolume(0);

        AudioClip audioClip = ResourceManager.Instance.LoadAsset<AudioClip>("", string.Format(AudioPath, name));
        if (audioClip != null)
        {
            if (!AudioSourceDic.ContainsKey(name))
            {
                AudioSourceDic.Add(name, Effect.AddComponent<AudioSource>());
            }
            if (AudioSourceDic[name].clip != null)
            {
                if (AudioSourceDic[name].clip.name != audioClip.name || !AudioSourceDic[name].isPlaying)
                {
                    AudioSourceDic[name].clip = audioClip;
                    AudioSourceDic[name].loop = false;
                    AudioSourceDic[name].volume = GetSoundEffectVolume();
                    AudioSourceDic[name].Play();
                }
            }
            else
            {
                AudioSourceDic[name].clip = audioClip;
                AudioSourceDic[name].loop = false;
                AudioSourceDic[name].volume = GetSoundEffectVolume();
                AudioSourceDic[name].Play();
            }
        }

    }

    public void PlaySoundMixEffect(string name, bool loop = false)
    {
        //if (!LocalDataManager.Instance.GetSoundState())
        //    SetSoundEffectVolume(0);
        AudioClip audioClip = ResourceManager.Instance.LoadAsset<AudioClip>("",string.Format(AudioPath, name));
        if (audioClip != null)
        {
            AudioSource audioSource = null;
            foreach (var source in AduioSourceList)
            {
                if (!source.isPlaying)
                {
                    audioSource = source;
                    break;
                }
            }
            if (audioSource == null)
            {
                audioSource = Effect.AddComponent<AudioSource>();
                AduioSourceList.Add(audioSource);
            }
            audioSource.clip = audioClip;
            audioSource.loop = false;
            audioSource.volume = GetSoundEffectVolume();
            audioSource.Play();
        }
    }

    public void StopSoundEffect()
    {
        Audio_Effect.Pause();
    }
    public void SoundEffectMute()
    {
        Audio_Effect.volume = 0;
    }
    public void SetSoundEffectVolume(float vule)
    {
        Audio_Effect.volume = vule;
    }

    public float GetSoundEffectVolume()
    {
        return Audio_Effect.volume;
    }
}
