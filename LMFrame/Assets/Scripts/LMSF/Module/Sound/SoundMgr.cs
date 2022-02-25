using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LMSF.Utils;

public class SoundMgr : MonoSingleton<SoundMgr>
{

    AudioSource Audio_Bgm;
    AudioSource Audio_Effect;

    string AudioPath = "Assets/MainApp/Audio/{0}.mp3";

    protected void Init()
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
        //AudioClip bgm = ResourcesManager.Load<AudioClip>(AudioPath + "bgm.wav");
        //Audio_Bgm.clip = bgm;
        //PlayBGM();
    }

    public void PlayBGM(string name = "bgm.wav",bool Loop=true)
    {

        //if (!LocalDataMgr.Instance.GetMusicState())
        //    SetBgmVolume(0);
        //AudioClip bgm = ResourcesManager.Load<AudioClip>(string.Format(AudioPath , name));
        //if (bgm != Audio_Bgm.clip)
        //{
        //    Audio_Bgm.clip = bgm;
        //    Audio_Bgm.loop = Loop;
        //    Audio_Bgm.Play();
        //}
    }
    public void RePlayBGM(string name = "bgm.wav", bool Loop = true)
    {

        //if (!LocalDataMgr.Instance.GetMusicState())
        //    SetBgmVolume(0);
        //AudioClip bgm = ResourcesManager.Load<AudioClip>(string.Format(AudioPath, name));
        //Audio_Bgm.clip = bgm;
        //Audio_Bgm.loop = Loop;
        //Audio_Bgm.Play();
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
    public void PlaySoundEffect(string name,bool Loop=false)
    {
        //if (!LocalDataMgr.Instance.GetSoundState())
        //    SetSoundEffectVolume(0);
        //AudioClip audioClip = ResourcesManager.Load<AudioClip>(string.Format(AudioPath , name));
        //Audio_Effect.clip = audioClip;
        //Audio_Effect.loop = Loop;
        //Audio_Effect.Play();
    }

    public void PlaySoundMixEffect(string name , bool loop = false)
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
