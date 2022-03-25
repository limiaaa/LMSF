using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LMSF.Utils;

public class SoundManager : MonoSingleton<SoundManager>
{
    AudioSource Audio_Bgm;
    AudioSource Audio_Effect;

    List<AudioClip> moduleList = null;
    Dictionary<string, List<AudioClip>> soundDic = new Dictionary<string, List<AudioClip>>();

    string AudioPath = "Assets/MainApp/Audio/{0}.mp3";
    public void Init()
    {
        ObjInit();
        AudioListener listener = transform.GetComponent<AudioListener>();
        if (listener == null)
            listener = gameObject.AddComponent<AudioListener>();
    }
    private void ObjInit()
    {
        GameObject Bgm = new GameObject("Audio_Bgm");
        Bgm.transform.SetParent(transform);
        Bgm.AddComponent<AudioSource>();
        Audio_Bgm = Bgm.GetComponent<AudioSource>();
        GameObject Effect = new GameObject("Audio_Effect");
        Effect.transform.SetParent(transform);
        Effect.AddComponent<AudioSource>();
        Audio_Effect = Effect.GetComponent<AudioSource>();
    }
    public void PlayBgm(string name = "bgm.wav", bool Loop = true, bool IsReplay = true)
    {
        //if (!LocalDataMgr.Instance.GetMusicState())
        //    SetBgmVolume(0);
        //AudioClip bgm = ResourcesManager.Load<AudioClip>(string.Format(AudioPath, name));
        //if (bgm != Audio_Bgm.clip || IsReplay)
        //{
        //    Audio_Bgm.clip = bgm;
        //    Audio_Bgm.loop = Loop;
        //    Audio_Bgm.Play();
        //}
    }
    public void StopBgm()
    {
        Audio_Bgm.Pause();
    }
    public void MuteBgm()
    {
        Audio_Bgm.volume = 0;
    }
    public void SetVolumeBgm(float vule)
    {
        Audio_Bgm.volume = vule;
    }
    public float GetVolumeBgm()
    {
        return Audio_Bgm.volume;
    }
    //同一时间只能存在一个
    public void PlaySoundEffect(string name,bool Loop=false,string moduleKey="")
    {
        //if (!LocalDataMgr.Instance.GetSoundState())
        //    SetSoundEffectVolume(0);
        //AudioClip audioClip = ResourcesManager.Load<AudioClip>(string.Format(AudioPath, name));
        AudioClip audioClip = null;
        Audio_Effect.clip = audioClip;
        Audio_Effect.loop = Loop;
        Audio_Effect.Play();
        if (moduleKey!="" && !soundDic.ContainsKey(moduleKey))
        {
            soundDic.Add(moduleKey, new List<AudioClip>());
        }
        soundDic[moduleKey].Add(audioClip);
    }
    //同一时间可以存在多个
    public void PlaySoundEffectWithPosition(string name , bool loop = false)
    {
        //if (!LocalDataMgr.Instance.GetSoundState())
        //    SetSoundEffectVolume(0);
        //AudioClip audioClip = ResourcesManager.Load<AudioClip>(string.Format(AudioPath , name));
        //AudioSource.PlayClipAtPoint(audioClip , transform.position , GetSoundEffectVolume());
    }
    public void StopSoundEffect()
    {
        Audio_Effect.Pause();
    }
    public void MuteSoundEffect()
    {
        Audio_Effect.volume = 0;
    }
    public void SetVolumeSoundEffect(float vule)
    {
        Audio_Effect.volume = vule;
    }
    public float GetVolumeSoundEffect()
    {
        return Audio_Effect.volume;
    }

    public void CloseSoundByModule(string moduleKey)
    {
        if (soundDic.ContainsKey(moduleKey))
        {
            soundDic.Remove(moduleKey);
        }
    }

}
