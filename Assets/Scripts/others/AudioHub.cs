using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHub : MonoBehaviour
{
    public static AudioHub Instance;
    void Awake(){ if (Instance && Instance!=this){ Destroy(gameObject); return; } Instance=this; DontDestroyOnLoad(gameObject); }

    [System.Serializable] public class NamedClip{ public string key; public AudioClip clip; }

    [Header("Clip Maps (拖进来)")]
    public List<NamedClip> bgmClips = new();
    public List<NamedClip> sfxClips = new();
    public List<NamedClip> voiceClips = new();

    Dictionary<string, AudioClip> _bgm, _sfx, _voice;

    [Header("Sources")]
    public AudioSource bgmSrc;    // 1 个
    public AudioSource voiceSrc;  // 1 个
    public AudioSource sfxPrefab; // 用于做池子

    [Header("SFX Pool")]
    public int sfxPoolSize = 8;
    readonly List<AudioSource> _sfxPool = new();

    [Header("Ducking")]
    public bool duckDuringVoice = true;
    public float duckVolume = 0.35f;
    public float duckFade = 0.15f;

    float bgmTargetVol = 1f;

    void Start(){
        _bgm   = BuildMap(bgmClips);
        _sfx   = BuildMap(sfxClips);
        _voice = BuildMap(voiceClips);

        // 建立 SFX 池
        if (sfxPrefab){
            for (int i=0;i<sfxPoolSize;i++){
                var a = Instantiate(sfxPrefab, transform);
                a.playOnAwake=false; a.loop=false;
                _sfxPool.Add(a);
            }
        }
    }

    Dictionary<string,AudioClip> BuildMap(List<NamedClip> list){
        var d = new Dictionary<string,AudioClip>(System.StringComparer.OrdinalIgnoreCase);
        foreach (var x in list) if (x!=null && !string.IsNullOrEmpty(x.key) && x.clip) d[x.key]=x.clip;
        return d;
    }

    // -------- BGM --------
    public void PlayBGM(string key, float vol=1f, float fade=0.8f, bool loop=true){
        if (!_bgm.TryGetValue(key, out var clip) || clip==null){ Debug.LogWarning($"[Audio] BGM '{key}' not found"); return; }
        StartCoroutine(FadeToBGM(clip, vol, fade, loop));
    }
    public void StopBGM(float fade=0.6f){ StartCoroutine(FadeOut(bgmSrc, fade)); }

    System.Collections.IEnumerator FadeToBGM(AudioClip clip, float vol, float fade, bool loop){
        if (bgmSrc==null) yield break;
        yield return FadeOut(bgmSrc, fade*0.5f);
        bgmSrc.clip=clip; bgmSrc.loop=loop; bgmSrc.volume=0f; bgmSrc.Play();
        bgmTargetVol = vol;
        yield return FadeIn(bgmSrc, vol, fade*0.5f);
    }

    // -------- SFX --------
    public void PlaySFX(string key, float vol=1f){
        if (!_sfx.TryGetValue(key, out var clip) || clip==null){ Debug.LogWarning($"[Audio] SFX '{key}' not found"); return; }
        var a = GetFreeSFX();
        a.clip=clip; a.volume=vol; a.Play();
    }
    AudioSource GetFreeSFX(){
        foreach (var a in _sfxPool) if (!a.isPlaying) return a;
        return _sfxPool[0]; // 全忙就抢一个
    }

    // -------- Voice --------
    public void PlayVoice(string key, float vol=1f, bool duck=true){
        if (voiceSrc==null){ Debug.LogWarning("[Audio] voiceSrc missing"); return; }
        if (!_voice.TryGetValue(key, out var clip) || clip==null){ Debug.LogWarning($"[Audio] VO '{key}' not found"); return; }
        StopVoice();
        voiceSrc.clip=clip; voiceSrc.volume=vol; voiceSrc.loop=false; voiceSrc.Play();
        if (duck && bgmSrc) StartCoroutine(DuckRoutine());
    }
    public void StopVoice(){ if (voiceSrc && voiceSrc.isPlaying) voiceSrc.Stop(); if (duckDuringVoice) SetDuck(false); }

    // -------- Ducking --------
    public void SetDuck(bool on){
        if (!bgmSrc) return;
        StopAllCoroutines(); // 只影响淡入淡出与duck
        StartCoroutine(on ? LerpVol(bgmSrc, bgmSrc.volume, duckVolume, duckFade)
                          : LerpVol(bgmSrc, bgmSrc.volume, bgmTargetVol, duckFade));
    }
    System.Collections.IEnumerator DuckRoutine(){
        if (!bgmSrc) yield break;
        yield return LerpVol(bgmSrc, bgmSrc.volume, duckVolume, duckFade);
        // 等语音结束
        while (voiceSrc && voiceSrc.isPlaying) yield return null;
        yield return LerpVol(bgmSrc, bgmSrc.volume, bgmTargetVol, duckFade);
    }

    // -------- fades --------
    System.Collections.IEnumerator FadeOut(AudioSource a, float t){ if (!a) yield break; float s=a.volume; for(float e=0;e<t; e+=Time.unscaledDeltaTime){ a.volume=Mathf.Lerp(s,0,e/t); yield return null;} a.volume=0; a.Stop();}
    System.Collections.IEnumerator FadeIn (AudioSource a, float v, float t){ if (!a) yield break; a.Play(); for(float e=0;e<t; e+=Time.unscaledDeltaTime){ a.volume=Mathf.Lerp(0,v,e/t); yield return null;} a.volume=v;}
    System.Collections.IEnumerator LerpVol(AudioSource a, float from, float to, float t){ if (!a){yield break;} for(float e=0;e<t; e+=Time.unscaledDeltaTime){ a.volume=Mathf.Lerp(from,to,e/t); yield return null;} a.volume=to; }

}
